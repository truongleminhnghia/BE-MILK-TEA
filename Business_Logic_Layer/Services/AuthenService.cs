using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BCrypt.Net;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories.Entities;
using Data_Access_Layer.Repositories.Interfaces;

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

        public async Task<string> Login(LoginRequest _request, string _type)
        {
            if(_type.Equals(TypeLogin.LOGIN_LOCAL.ToString())) {
                Account? _existingAccount =  await _accountRepository.GetByEmail(_request.Email);
                if(_existingAccount == null) {
                    throw new Exception("Email or Password Invalid");
                }
                bool checkPassword = _passwordHasher.VerifyPassword(_request.Password, _existingAccount.Password);
                if(checkPassword) {
                    return _jwtService.GenerateJwtToken(_existingAccount);
                }
            }
            throw new Exception("Login failed");
        }

        public async Task<AccountResponse> Register(RegisterRequest _request, string _type)
        {
            bool isAdmin = !_type.Equals(TypeLogin.LOGIN_LOCAL);
            if (isAdmin)
            {
                if (_type.Equals(TypeLogin.LOGIN_LOCAL.ToString()))
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
            }
            throw new Exception("Đăng ký thất bại");
        }
    }
}