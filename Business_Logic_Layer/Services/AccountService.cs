using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.AutoMappers;
using Business_Logic_Layer.Models.Requests;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories.Entities;
using Data_Access_Layer.Repositories.Interfaces;

namespace Business_Logic_Layer.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        } 
        public async Task<Account> Register(RegisterRequest _request)
        {
            var existingAccountByEmail = await _accountRepository.GetByEmail(_request.Email);
            if (existingAccountByEmail != null)
            {
                throw new ArgumentException("Email already exists");
            }
            Account account = _mapper.Map<Account>(_request);
            account.AccountStatus = AccountStatus.ACTIVE;
            account.RoleName = RoleName.ROLE_CUSTOMER;
            return await _accountRepository.Create(account);
        }
    }
}