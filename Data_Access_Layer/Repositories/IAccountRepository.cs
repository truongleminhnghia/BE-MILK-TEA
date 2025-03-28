using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;

namespace Data_Access_Layer.Repositories
{
    public interface IAccountRepository
    {
        Task<Account> Create(Account _account);
        Task<Account?> GetById(Guid _id);
        Task<Account?> GetByEmail(string _email);
        public Task<bool> EmailExisting(string _email);
        Task<Account> GetByPhoneNumber(string phoneNumber);
        Task UpdateAccount(Account account);
        Task<Account> GetAccountByOrderIdAsync(Guid orderId);
        Task<(IEnumerable<Account>, int TotalCount)> GetAllAccountsAsync(
    string? search, AccountStatus? accountStatus, RoleName? roleName,
    string? sortBy, bool isDescending, int page, int pageSize);
        Task<bool> UpdateCustomerAccountLevel(Guid accountId);
    }
}

