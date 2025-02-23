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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Google.Apis.Auth;

namespace Business_Logic_Layer.Services
{
    public class AuthenService : IAuthenService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        
        private readonly IJwtService _jwtService;
        public AuthenService(IAccountRepository accountRepository, IMapper mapper, IPasswordHasher passwordHasher, IJwtService jwtService)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public Task<string> GenerateUrl(string _type)
        {
            throw new NotImplementedException();
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
                        throw new Exception("Account does not exist");
                    }
                    bool checkPassword = _passwordHasher.VerifyPassword(_request.Password, _account.Password);
                    if (checkPassword)
                    {
                        _token = _jwtService.GenerateJwtToken(_account);
                    }
                    else
                    {
                        throw new Exception("Invalid password");
                    }
                }
                else if (_type.Trim().Equals(TypeLogin.LOGIN_GOOGLE.ToString()))
                {
                    // Assuming _request.Email is already verified by Google
                    _account = await _accountRepository.GetByEmail(_request.Email);
                    if (_account == null)
                    {
                        // Register new account if it doesn't exist
                        var registerRequest = new RegisterRequest
                        {
                            Email = _request.Email,
                            FirstName = "GoogleUser", // Default value, should be replaced with actual data
                            LastName = "GoogleUser",  // Default value, should be replaced with actual data
                            Password = Guid.NewGuid().ToString(), // Random password, not used
                            PhoneNumber = "0000000000" // Default value, should be replaced with actual data
                        };
                        _account = _mapper.Map<Account>(registerRequest);
                        _account.AccountStatus = AccountStatus.ACTIVE;
                        _account.RoleName = RoleName.ROLE_CUSTOMER;
                        await _accountRepository.Create(_account);
                    }
                    _token = _jwtService.GenerateJwtToken(_account);
                }
                else
                {
                    throw new Exception("Invalid login type");
                }

                AccountResponse _accountResponse = _mapper.Map<AccountResponse>(_account);
                _authenticateResponse = new AuthenticateResponse(_token, _accountResponse);

                return _authenticateResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return new AuthenticateResponse("", new AccountResponse());
            }
        }

        //public async Task<AccountResponse> Register(RegisterRequest _request, string _type)
        //{
        //    bool isAdmin = !_type.Equals(TypeLogin.LOGIN_LOCAL);
        //    if (isAdmin)
        //    {
        //        if (_type.Equals(TypeLogin.LOGIN_LOCAL.ToString()))
        //        {
        //            var existingEmail = await _accountRepository.GetByEmail(_request.Email);
        //            if (existingEmail != null)
        //            {
        //                throw new Exception("Email đã tồn tại.");
        //            }
        //            Account _account = _mapper.Map<Account>(_request);
        //            _account.Password = _passwordHasher.HashPassword(_request.Password);
        //            _account.AccountStatus = AccountStatus.AWAITING_CONFIRM;
        //            _account.RoleName = RoleName.ROLE_CUSTOMER;
        //            await _accountRepository.Create(_account);
        //            return _mapper.Map<AccountResponse>(_account);
        //        }
        //    }
        //    throw new Exception("Đăng ký thất bại");
        //}

        public async Task<AccountResponse> Register(RegisterRequest request)
        {
            try
            {
                foreach (var item in request.GetType().GetProperties())
                {
                    if (item.GetValue(request) == null)
                    {
                        throw new Exception("Please fill in all fields");
                    }
                }

                var existingEmail = await _accountRepository.GetByEmail(request.Email);
                if (existingEmail != null)
                {
                    throw new Exception("Email already exists");
                }
                var existingPhoneNumber = await _accountRepository.GetByPhoneNumber(request.PhoneNumber);
                if (existingPhoneNumber != null)
                {
                    throw new Exception("Phone number already exists");
                }

                var newAccount = _mapper.Map<Account>(request);
                newAccount.CreateAt = DateTime.Now;
                newAccount.Password = _passwordHasher.HashPassword(request.Password);
                newAccount.AccountStatus = AccountStatus.AWAITING_CONFIRM;
                switch (newAccount.RoleName)
                {
                    case RoleName.ROLE_ADMIN:
                        newAccount.AccountStatus = AccountStatus.ACTIVE;
                        break;
                    case RoleName.ROLE_CUSTOMER:
                        newAccount.AccountStatus = AccountStatus.AWAITING_CONFIRM;
                        break;
                    case RoleName.ROLE_STAFF:
                        newAccount.AccountStatus = AccountStatus.AWAITING_CONFIRM;
                        break;
                    case RoleName.ROLE_MANAGER:
                        newAccount.AccountStatus = AccountStatus.AWAITING_CONFIRM;
                        break;
                    default:
                        throw new Exception("Role is not valid");
                }
                await _accountRepository.Create(newAccount);
                return _mapper.Map<AccountResponse>(newAccount);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //public async Task<ApiResponse> Logout()
        //{
        //    try
        //    {
        //        await _signInManager.SignOutAsync();
        //        return new ApiResponse(HttpStatusCode.OK, true, "Logout successful");
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponse(HttpStatusCode.InternalServerError, false, ex.Message);
        //    }
        //}

        public async Task<ApiResponse> LoginWithGoogle(string googleToken)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken);
            var user = await _accountRepository.GetByEmail(payload.Email);

            if (user == null)
            {
                user = new Account
                {
                    Email = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    Password = "", // No password needed for Google sign-in
                    CreateAt = DateTime.UtcNow,
                    AccountStatus = AccountStatus.ACTIVE,
                    RoleName = RoleName.ROLE_CUSTOMER
                };

                await _accountRepository.Create(user);
            }

            var token = _jwtService.GenerateJwtToken(user);
            return new ApiResponse(HttpStatusCode.OK, true, "Login Google successful", token);
        }
        //public async Task InvalidateUserTokensAsync(Guid userId)
        //{
        //    var activeTokens = await .Tokens
        //        .Where(t => t.UserId == userId &&
        //                   t.ExpiryDate > DateTime.UtcNow &&
        //                   !t.IsInvalidated)
        //        .ToListAsync();

        //    foreach (var token in activeTokens)
        //    {
        //        token.IsInvalidated = true;
        //    }

        //    await _context.SaveChangesAsync();
        //}
    }
}