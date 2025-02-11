using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Repositories.Entities;
using Data_Access_Layer.Repositories.Interfaces;

namespace Business_Logic_Layer.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<Account> Register(Account _account)
        {
            var existingAccountByEmail = await _accountRepository.GetByEmail(_account.Email);
            if (existingAccountByEmail != null)
            {
                throw new ArgumentException("Email already exists");
            }
            
            return await _accountRepository.Create(_account);
        }
    }
}