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
using Business_Logic_Layer.Utils;
using Data_Access_Layer.Repositories;
using System.Text.Unicode;
using System.Web;
using Azure.Core;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Identity.Client;

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
                    throw new Exception("Id bị thiếu");
                }
                var _account = await _accountRepository.GetByEmail(_email);
                if (_account == null)
                {
                    throw new Exception("Tài khoản không tồn tại");
                }
                return _account;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }

        public async Task<AccountResponse?> GetById(Guid id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Id bị thiếu");
                }
                var _account = await _accountRepository.GetById(id);
                if (_account == null)
                {
                    throw new Exception("Tài khoản không tồn tại");
                }
                return MapToAccountResponse.ComplexAccountResponse(_account);
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
                var existingEmail = await _accountRepository.GetByEmail(createAccountRequest.Email);
                if (existingEmail != null)
                {
                    throw new Exception("Email đã tồn tại.");
                }

                var account = _mapper.Map<Account>(createAccountRequest);
                account.AccountStatus = AccountStatus.ACTIVE;

                var currentAccount = await _accountRepository.GetById(_source.GetCurrentAccount());
                if (currentAccount.RoleName == RoleName.ROLE_ADMIN)
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
                    throw new Exception("Tạo tài khoản thất bại");
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
        //        var result = await _accountRepository.Create(_account);
        //        return result;
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
        //        var result = await _accountRepository.Create(_account);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("error: " + ex.Message);
        //        return null;
        //    }
        //}

        public async Task<Account?> UpdateAccount(Guid id, UpdateAccountRequest request)
        {
            try
            {
                var account = await _accountRepository.GetById(id);
                if (account == null) return null;

                // Cập nhật thông tin Account
                if (!string.IsNullOrEmpty(request.FirstName)) account.FirstName = request.FirstName;
                if (!string.IsNullOrEmpty(request.LastName)) account.LastName = request.LastName;
                if (!string.IsNullOrEmpty(request.Password)) account.Password = request.Password;

                // Cập nhật thông tin Customer nếu có
                if (account.Customer != null && request.Customer != null)
                {
                    account.Customer.TaxCode = request.Customer.TaxCode ?? account.Customer.TaxCode;
                    account.Customer.Address = request.Customer.Address ?? account.Customer.Address;
                }

                // Cập nhật thông tin Employee nếu có
                if (account.Employee != null && request.Employee != null)
                {
                    account.Employee.RefCode = request.Employee.RefCode ?? account.Employee.RefCode;
                }

                await _accountRepository.UpdateAccount(account);
                return account;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }


        //public async Task<IEnumerable<AccountResponse>> GetAllAccount()
        //{
        //    try
        //    {
        //        var accounts = await _accountRepository.GetAllAccount();
        //        var accountResponses = accounts.Select(account =>
        //        {
        //            var response = _mapper.Map<AccountResponse>(account);

        //            // Ánh xạ Employee nếu có
        //            if (account.Employee != null)
        //            {
        //                response.Employee = _mapper.Map<EmployeeResponse>(account.Employee);
        //            }

        //            // Ánh xạ Customer nếu có
        //            if (account.Customer != null)
        //            {
        //                response.Customer = _mapper.Map<CustomerResponse>(account.Customer);
        //            }

        //            return response;
        //        });

        //        return accountResponses;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("error: " + ex.Message);
        //        return null;
        //    }
        //}

        public async Task<IEnumerable<Account>> GetAllAccounts(
            string? search, string? sortBy, bool isDescending,
            AccountStatus? accountStatus, RoleName? role, int page, int pageSize)
        {
            return await _accountRepository.GetAllAccounts(
                search, sortBy, isDescending, accountStatus, role, page, pageSize);
        }

    }
}

// đk -> GG, FB, LOCAL (params) (GG, FB ko sử dụng MK, và mặc định với ROLE_CUSTOMER) 
// khi nó là LOCAL phải check ROLE là ADMIN hay ko phải ADMIN, nếu là ADMIN mở full ROLE nếu ko PHẢI ADMIN thì mặc định nó ra user