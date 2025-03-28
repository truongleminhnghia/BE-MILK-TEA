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
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

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

                //kiem tra xem user dang dang nhap co phai la admin hay khong
                //neu khong phai thi se tra ve thong tin cua user do
                Account account;
                //var currentUser = await _source.GetCurrentAccount();
                //if (currentUser.RoleName != RoleName.ROLE_ADMIN)
                //{
                //    account = await _accountRepository.GetById(currentUser.Id);
                //    if (account == null)
                //    {
                //        throw new Exception("Tài khoản không tồn tại");
                //    }
                //    return MapToAccountResponse.ComplexAccountResponse(account);
                //}

                account = await _accountRepository.GetById(id);
                if (account == null)
                {
                    throw new Exception("Tài khoản không tồn tại");
                }
                return MapToAccountResponse.ComplexAccountResponse(account);
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
                if (!string.IsNullOrEmpty(request.PhoneNumber)) account.Phone = request.PhoneNumber;
                if (!string.IsNullOrEmpty(request.ImgUrl)) account.ImageUrl = request.ImgUrl;

                // Cập nhật thông tin Customer nếu có
                if (account.Customer != null && request.Customer != null && account.RoleName == RoleName.ROLE_CUSTOMER)
                {
                    account.Customer.TaxCode = request.Customer.TaxCode ?? account.Customer.TaxCode;
                    account.Customer.Address = request.Customer.Address ?? account.Customer.Address;
                }
                else
                {
                    throw new ArgumentException("Loại tài khoản không hợp lệ.");
                }

                // BO CAP NHAT REFCODE CHO EMPLOYEE
                // Cập nhật thông tin Employee nếu có
                //if (account.Employee != null && request.Employee != null && account.RoleName == RoleName.ROLE_STAFF)
                //{
                //    account.Employee.RefCode = request.Employee.RefCode ?? account.Employee.RefCode;
                //}
                account.UpdateAt = DateTime.UtcNow;
                await _accountRepository.UpdateAccount(account);
                return account;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }

        public async Task<PageResult<AccountResponse>> GetAllAccountsAsync(
    string? search, AccountStatus? accountStatus, RoleName? roleName,
    string? sortBy, bool isDescending, int page, int pageSize)
        {
            var (accounts, total) = await _accountRepository.GetAllAccountsAsync(
                search, accountStatus, roleName, sortBy, isDescending, page, pageSize);

            if (accounts.IsNullOrEmpty())
            {
                throw new Exception("Không tìm thấy danh sach tài khoản nào");
            }

            return new PageResult<AccountResponse>
            {
                Data = _mapper.Map<List<AccountResponse>>(accounts),
                PageCurrent = page,
                PageSize = pageSize,
                Total = total
            };
        }


        public async Task<bool> UpdateAccountLevel(Guid accountId, AccountLevelEnum newLevel)
        {
            try
            {
                return await _accountRepository.UpdateCustomerAccountLevel(accountId, newLevel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi trong quá trình cập nhật Account Level: {ex.Message}");
                return false;

        public async Task<AccountResponse> DeleteAccount(Guid id)
        {
            try
            {
                var account = await _accountRepository.GetById(id);
                if (account == null) throw new Exception("Không tìm thấy tài khoản");
                await _accountRepository.DeleteAsync(id);
                return MapToAccountResponse.ComplexAccountResponse(account);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi ở DeleteAccount: " + ex.Message);
                throw;

            }
        }
    }
}

// đk -> GG, FB, LOCAL (params) (GG, FB ko sử dụng MK, và mặc định với ROLE_CUSTOMER) 
// khi nó là LOCAL phải check ROLE là ADMIN hay ko phải ADMIN, nếu là ADMIN mở full ROLE nếu ko PHẢI ADMIN thì mặc định nó ra user