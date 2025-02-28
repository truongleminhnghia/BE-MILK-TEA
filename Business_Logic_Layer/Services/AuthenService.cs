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

        public async Task<AccountResponse> Register(RegisterRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                var currentUser = _source.GetCurrentAccount();

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

                    account.Customer.AccountId = account.Id;
                    account.Customer.TaxCode = null;
                    account.Customer.Address = null;
                    await _customerRepository.Create(account.Customer);
                }

                if(currentUser.RoleName == RoleName.ROLE_ADMIN)
                {
                    account.Password = _passwordHasher.HashPassword(request.Password);
                    account.AccountStatus = AccountStatus.ACTIVE;
                    await _accountRepository.Create(account);

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

                await transaction.CommitAsync();

                return MapToAccountResponse.ComplexAccountResponse(account);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }

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