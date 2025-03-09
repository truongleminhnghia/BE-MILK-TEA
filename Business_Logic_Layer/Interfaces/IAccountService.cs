using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Interfaces
{
    public interface IAccountService
    {
        public Task<AccountResponse?> GetById(Guid _id);
        public Task<Account?> GetByEmail(string _email);

        Task<Account> UpdateAccount(Guid id, UpdateAccountRequest updateAccountRequest);
        Task<IEnumerable<Account>> GetAllAccounts(string? search, string? sortBy, bool isDescending, AccountStatus? accountStatus, RoleName? role, int page, int pageSize);


    }
}