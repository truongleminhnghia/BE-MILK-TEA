using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BCrypt.Net;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Entities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Data_Access_Layer.Repositories;
using System.Web;
using System.Text.Json;
using System.Net.Http.Headers;
using Business_Logic_Layer.Utils;
using Data_Access_Layer.Data;
using System.Security.Principal;
using Azure.Core;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;

namespace Business_Logic_Layer.Services
{
    public class AuthenService : IAuthenService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ApplicationDbContext _context;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmployeeRepository _employeeRepository;

        private string _clientIdGoogle = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
        private string _clientSecretGoogle = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET_KEY");
        private string _redirectUriGoogle = Environment.GetEnvironmentVariable("GOOGLE_REDIRECT_URI");
        private string _scopesGoogle = Environment.GetEnvironmentVariable("GOOGLE_SCOPE");
        private string _googleUserInfo = Environment.GetEnvironmentVariable("GOOGLE_INFO_URI");
        private string _flowNameGoogle = "flowName=GeneralOAuthFlow";

        private readonly IJwtService _jwtService;
        private readonly Source _source;
        private readonly HttpClient _httpClient;
        public AuthenService(IAccountRepository accountRepository, IMapper mapper, IPasswordHasher passwordHasher, IJwtService jwtService, HttpClient httpClient, Source source, ApplicationDbContext applicationDbContext, ICustomerRepository customerRepository, IEmployeeRepository employeeRepository)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _httpClient = httpClient;
            _source = source;
            _context = applicationDbContext;
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<AuthenticateResponse> LoginLocal(LoginRequest request, string type)
        {
            try
            {
                AuthenticateResponse authenticateResponse = null;
                Account account;
                string token = "";

                if (type.Trim().IsNullOrEmpty() || type.Equals(TypeLogin.LOGIN_LOCAL.ToString()))
                {
                    account = await _accountRepository.GetByEmail(request.Email);
                    if (account == null)
                    {
                        throw new Exception("Account does not exist");
                    }
                    bool checkPassword = _passwordHasher.VerifyPassword(request.Password, account.Password);
                    if (checkPassword)
                    {
                        token = _jwtService.GenerateJwtToken(account);
                    }
                    else
                    {
                        throw new Exception("Invalid password");
                    }
                }
                else if (type.Trim().Equals(TypeLogin.LOGIN_GOOGLE.ToString()))
                {
                    // Assuming _request.Email is already verified by Google
                    account = await _accountRepository.GetByEmail(request.Email);
                    if (account == null)
                    {
                        // Register new account if it doesn't exist
                        var registerRequest = new RegisterRequest
                        {
                            Email = request.Email,
                            FirstName = "GoogleUser", // Default value, should be replaced with actual data
                            LastName = "GoogleUser",  // Default value, should be replaced with actual data
                            Password = Guid.NewGuid().ToString(), // Random password, not used
                        };
                        account = _mapper.Map<Account>(registerRequest);
                        account.AccountStatus = AccountStatus.ACTIVE;
                        account.RoleName = RoleName.ROLE_CUSTOMER;
                        await _accountRepository.Create(account);
                    }
                    token = _jwtService.GenerateJwtToken(account);
                }
                else
                {
                    throw new Exception("Invalid login type");
                }

                AccountResponse _accountResponse = _mapper.Map<AccountResponse>(account);
                authenticateResponse = new AuthenticateResponse(token, _accountResponse);

                return authenticateResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return new AuthenticateResponse("", new AccountResponse());
            }
        }

        public async Task<AccountResponse> Register(CreateAccountRequest request)
        {
            var strategy = _context.Database.CreateExecutionStrategy(); // Lấy chiến lược thực thi an toàn
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {

                    Account? currentUser = null;

                    try
                    {
                        currentUser = await _source.GetCurrentAccount();
                    }
                    catch (Exception ex)
                    {
                        // Ghi log lỗi thay vì cho lỗi này rơi thẳng vào catch chính
                        Console.WriteLine("Lỗi khi lấy CurrentUser: " + ex.Message);
                    }

                    var existingEmail = await _accountRepository.GetByEmail(request.Email);
                    if (existingEmail != null)
                    {
                        throw new Exception("Email đã tồn tại.");
                    }
                    Account account = _mapper.Map<Account>(request);
                    if (currentUser == null)
                    {
                        account.Password = _passwordHasher.HashPassword(request.Password);
                        account.AccountStatus = AccountStatus.ACTIVE;
                        account.RoleName = RoleName.ROLE_CUSTOMER;
                        await _accountRepository.Create(account);

                        if (account.Customer == null)
                        {
                            account.Customer = new Customer();
                        }
                        account.Customer.AccountId = account.Id;
                        account.Customer.TaxCode = string.Empty;
                        account.Customer.Address = string.Empty;
                        await _customerRepository.Create(account.Customer);
                    }

                    if (currentUser?.RoleName == RoleName.ROLE_ADMIN)
                    {
                        account.Password = _passwordHasher.HashPassword(request.Password);
                        account.AccountStatus = AccountStatus.ACTIVE;
                        await _accountRepository.Create(account);

                        if (account.Employee == null)
                        {
                            account.Employee = new Employee();
                        }
                        account.Employee.AccountId = account.Id;
                        bool isUniqueRefCode; // bool isUniqueRefCode = true;
                        do
                        {
                            account.Employee.RefCode = account.Employee.RefCode = _source.GenerateRandom8Digits().ToString();
                            isUniqueRefCode = await _employeeRepository.CheckRefCode(account.Employee.RefCode);
                        }
                        while (!isUniqueRefCode);
                        await _employeeRepository.Create(account.Employee);
                    }
                    await transaction.CommitAsync(); // Commit transaction khi mọi thứ thành công
                    return MapToAccountResponse.ComplexAccountResponse(account);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(); // Nếu có lỗi, rollback tất cả
                    throw new Exception("Error: " + ex.Message);
                }
            });

        }
        public string GenerateUrl(string _type)
        {
            if (_type.Equals(TypeLogin.LOGIN_GOOGLE.ToString()))
            {
                string _state = Guid.NewGuid().ToString();
                string _nonce = Guid.NewGuid().ToString();
                string _encodestate = HttpUtility.UrlEncode(_state);
                string _encodeNonce = HttpUtility.UrlEncode(_nonce);

                string _url = "https://accounts.google.com/o/oauth2/v2/auth/oauthchooseaccount?" +
                                "response_type=code&" +
                                "client_id=" + _clientIdGoogle + "&" +
                                "scope=" + _scopesGoogle + "&" +
                                "state=" + _encodestate + "&" +
                                "nonce=" + _encodeNonce + "&" +
                                "redirect_uri=" + _redirectUriGoogle + "&" + _flowNameGoogle;
                return _url;
            }
            return null;
        }

        public async Task<Dictionary<string, object>> AuthenticateAndFetchProfile(string code, string type)
        {
            var tokenUrl = "https://oauth2.googleapis.com/token";
            var requestData = new Dictionary<string, string>
        {
            {"code", code},
            {"client_id", _clientIdGoogle},
            {"client_secret", _clientSecretGoogle},
            {"redirect_uri", _redirectUriGoogle},
            {"grant_type", "authorization_code"}
        };

            var response = await _httpClient.PostAsync(tokenUrl, new FormUrlEncodedContent(requestData));
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);
            var accessToken = tokenResponse["access_token"].ToString();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var userInfoResponse = await _httpClient.GetStringAsync(_googleUserInfo);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(userInfoResponse);
        }

        public async Task<AuthenticateResponse> LoginOauth2(Oauth2Request _request)
        {
            var result = await _accountRepository.GetByEmail(_request.Email);
            if (result == null)
            {
                result = new Account
                {
                    Email = _request.Email,
                    FirstName = _request.FullName,
                    Phone = _request.PhoneNumber,
                    RoleName = RoleName.ROLE_CUSTOMER,
                };
                await _accountRepository.Create(result);
            }
            string token = _jwtService.GenerateJwtToken(result);
            AccountResponse _accountResponse = _mapper.Map<AccountResponse>(result);
            AuthenticateResponse _authenticateResponse = new AuthenticateResponse(
                         token,
                         _accountResponse
                     );
            return _authenticateResponse;
        }
    }
}