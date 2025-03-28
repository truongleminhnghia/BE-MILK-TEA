using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Services
{
    public interface IAccountService
    {
        public Task<AccountResponse?> GetById(Guid _id);
        public Task<Account?> GetByEmail(string _email);

        Task<Account> UpdateAccount(Guid id, UpdateAccountRequest updateAccountRequest);
        Task<PageResult<AccountResponse>> GetAllAccountsAsync(
    string? search, AccountStatus? accountStatus, RoleName? roleName,
    string? sortBy, bool isDescending, int page, int pageSize);

        Task<bool> UpdateAccountLevel(Guid accountId, AccountLevelEnum newLevel);
    }
}