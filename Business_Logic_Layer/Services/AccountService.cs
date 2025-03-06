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
        private readonly ICartRepository _cartRepository;

        public AccountService(IAccountRepository accountRepository, IMapper mapper, Source source, ICartRepository cartRepository)
        {
            _accountRepository = accountRepository;
            _cartRepository = cartRepository;
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
                if (account.Customer != null && request.Customer != null && account.RoleName == RoleName.ROLE_CUSTOMER)
                {
                    account.Customer.TaxCode = request.Customer.TaxCode ?? account.Customer.TaxCode;
                    account.Customer.Address = request.Customer.Address ?? account.Customer.Address;
                }

                // Cập nhật thông tin Employee nếu có
                if (account.Employee != null && request.Employee != null && account.RoleName == RoleName.ROLE_STAFF)
                {
                    account.Employee.RefCode = request.Employee.RefCode ?? account.Employee.RefCode;
                }
                //var cart = new Cart
                //{
                //    Id = Guid.NewGuid(),        // Tạo ID mới cho Cart
                //    AccountId = account.Id,    // Gán ID tài khoản cho Cart
                //    CreatedAt = DateTime.UtcNow
                //};

                //await _cartRepository.Create(cart); // Lưu giỏ hàng vào database


                await _accountRepository.UpdateAccount(account);
                return account;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }

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