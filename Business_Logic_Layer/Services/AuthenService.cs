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
using Data_Access_Layer.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Web;
using System.Text.Json;
using System.Net.Http.Headers;

namespace Business_Logic_Layer.Services
{
    public class AuthenService : IAuthenService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        private string _clientIdGoogle = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
        private string _clientSecretGoogle = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET_KEY");
        private string _redirectUriGoogle = Environment.GetEnvironmentVariable("GOOGLE_REDIRECT_URI");
        private string _scopesGoogle = Environment.GetEnvironmentVariable("GOOGLE_SCOPE");
        private string _googleUserInfo = Environment.GetEnvironmentVariable("GOOGLE_INFO_URI");
        private string _flowNameGoogle = "flowName=GeneralOAuthFlow";

        private readonly IJwtService _jwtService;

        private readonly HttpClient _httpClient;
        public AuthenService(IAccountRepository accountRepository, IMapper mapper, IPasswordHasher passwordHasher, IJwtService jwtService, HttpClient httpClient)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _httpClient = httpClient;
        }

        public async Task<AuthenticateResponse> Login(LoginRequest _request, string _type)
        {
            try
            {
                AuthenticateResponse _authenticateResponse = null;
                Account _account;
                string _token = "";
                if (_type.Trim().IsNullOrEmpty() || _type.Equals(TypeLogin.LOGIN_LOCAL.ToString()))
                {
                    _account = await _accountRepository.GetByEmail(_request.Email);
                    if (_account == null)
                    {
                        throw new Exception("Account do not existing");
                    }
                    bool checkPassword = _passwordHasher.VerifyPassword(_request.Password, _account.Password);
                    if (checkPassword)
                    {
                        _token = _jwtService.GenerateJwtToken(_account);
                    }
                    AccountResponse _accountResponse = _mapper.Map<AccountResponse>(_account);
                    _authenticateResponse = new AuthenticateResponse(
                        _token,
                        _accountResponse
                    );
                }
                else if (_type.Trim().Equals(TypeLogin.LOGIN_GOOGLE.ToString()))
                {
                    throw new Exception("Chua DEMO");
                }
                return _authenticateResponse ?? new AuthenticateResponse("", new AccountResponse());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return new AuthenticateResponse("", new AccountResponse());
            }
        }

        public async Task<AccountResponse> Register(RegisterRequest _request)
        {
            bool isAdmin = false;
            if (isAdmin)
            {
                    var existingEmail = await _accountRepository.GetByEmail(_request.Email);
                    if (existingEmail != null)
                    {
                        throw new Exception("Email đã tồn tại.");
                    }
                    Account _account = _mapper.Map<Account>(_request);
                    _account.Password = _passwordHasher.HashPassword(_request.Password);
                    _account.AccountStatus = AccountStatus.AWAITING_CONFIRM;
                    _account.RoleName = RoleName.ROLE_CUSTOMER;
                    await _accountRepository.Create(_account);
                    return _mapper.Map<AccountResponse>(_account);
            }
            throw new Exception("Đăng ký thất bại");
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