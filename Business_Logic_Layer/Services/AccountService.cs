using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.AutoMappers;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

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

        public Task<AccountResponse> Create(RegisterRequest _request)
        {
            throw new NotImplementedException();
        }

        public async Task<AccountResponse> Register(RegisterRequest _request)
        {
            // var existingAccountByEmail = await _accountRepository.GetByEmail(_request.Email);
            // if (existingAccountByEmail != null)
            // {
            //     throw new ArgumentException("Email already exists");
            // }
            Account account = _mapper.Map<Account>(_request);
            account.AccountStatus = AccountStatus.ACTIVE;
            account.RoleName = RoleName.ROLE_CUSTOMER;
            await _accountRepository.Create(account);
            return _mapper.Map<AccountResponse>(account);
        }
    }
}

// đk -> GG, FB, LOCAL (params) (GG, FB ko sử dụng MK, và mặc định với ROLE_CUSTOMER) 
// khi nó là LOCAL phải check ROLE là ADMIN hay ko phải ADMIN, nếu là ADMIN mở full ROLE nếu ko PHẢI ADMIN thì mặc định nó ra user