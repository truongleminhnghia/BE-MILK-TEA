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
using Business_Logic_Layer.Interfaces;
using Business_Logic_Layer.Utils;
using Data_Access_Layer.Repositories;
using System.Text.Unicode;
using System.Web;

namespace Business_Logic_Layer.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly Source _source;

        public AccountService(IAccountRepository accountRepository, IMapper mapper, Source source)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _source = source;
        }

        public async Task<Account?> GetByEmail(string _email)
        {
            try
            {
                if (_email == null)
                {
                    throw new Exception("Id do not null");
                }
                var _account = await _accountRepository.GetByEmail(_email);
                if (_account == null)
                {
                    throw new Exception("Account do not exits");
                }
                return _account;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }

        public async Task<Account?> GetById(Guid _id)
        {
            try
            {
                if (_id == null)
                {
                    throw new Exception("Id do not null");
                }
                var _account = await _accountRepository.GetById(_id);
                if (_account == null)
                {
                    throw new Exception("Account do not exits");
                }
                return _account;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }

        public async Task<Account> CreateAccount(CreateAccountRequest createAccountRequest)
        {
            try
            {
                foreach (var item in createAccountRequest.GetType().GetProperties())
                {
                    if (item.GetValue(createAccountRequest) == null)
                    {
                        throw new Exception("Request do not null!");
                    }
                }
                var account = _mapper.Map<Account>(createAccountRequest);
                account.AccountStatus = AccountStatus.AWAITING_CONFIRM;

                var currentAccount = await _accountRepository.GetById(_source.GetCurrentAccount());
                if (currentAccount == null)
                {
                    throw new Exception("Account do not exist");
                }
                else if (currentAccount.RoleName == RoleName.ROLE_ADMIN)
                {
                    //dien role name cho account moi
                    if (account.RoleName == RoleName.ROLE_STAFF || account.RoleName == RoleName.ROLE_MANAGER || account.RoleName == RoleName.ROLE_ADMIN)
                    {
                        account.AccountStatus = AccountStatus.ACTIVE;
                    }
                }
                else
                {
                    account.RoleName = RoleName.ROLE_CUSTOMER;
                }
                    var result = await _accountRepository.Create(account);
                if (result == null)
                {
                    throw new Exception("Create account failed");
                }

                return account;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
              

        //public async Task<Employee> CreateStaff(CreateStaffRequest createEmployeeRequest)
        //{
        //    try
        //    {
        //        if (createEmployeeRequest == null)
        //        {
        //            throw new Exception("Request do not null!");
        //        }
        //        var _account = _mapper.Map<Account>(createEmployeeRequest);
        //        _account.RoleName = RoleName.ROLE_STAFF;
        //        _account.AccountStatus = AccountStatus.ACTIVE;
        //        var _result = await _accountRepository.Create(_account);
        //        return _result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("error: " + ex.Message);
        //        return null;
        //    }
        //}

        //public async Task<Account> CreateManager(CreateManagerRequest createManagerRequest)
        //{
        //    try
        //    {
        //        if (createManagerRequest == null)
        //        {
        //            throw new Exception("Request do not null!");
        //        }
        //        var _account = _mapper.Map<Account>(createManagerRequest);
        //        _account.RoleName = RoleName.ROLE_MANAGER;
        //        _account.AccountStatus = AccountStatus.ACTIVE;
        //        _account
        //        var _result = await _accountRepository.Create(_account);
        //        return _result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("error: " + ex.Message);
        //        return null;
        //    }
        //}
    }
}

// đk -> GG, FB, LOCAL (params) (GG, FB ko sử dụng MK, và mặc định với ROLE_CUSTOMER) 
// khi nó là LOCAL phải check ROLE là ADMIN hay ko phải ADMIN, nếu là ADMIN mở full ROLE nếu ko PHẢI ADMIN thì mặc định nó ra user