using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Account> Create(Account _account)
        {
            _context.Accounts.AddAsync(_account);
            await _context.SaveChangesAsync();
            return _account;
        }

        public async Task<bool> EmailExisting(string _email)
        {
            var email = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == _email);
            return email == null;
        }

        public async Task<Account?> GetByEmail(string _email)
        {
            Account response;
            try
            {
                response = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == _email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return response;
        }

        public async Task<Account?> GetById(Guid id)
        {
            return await _context.Accounts
                .Include(a => a.Employee)
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Account> GetByPhoneNumber(string phoneNumber)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Phone == phoneNumber);
        }

        public async Task UpdateAccount(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<Account>, int TotalCount)> GetAllAccountsAsync(
    string? search, AccountStatus? accountStatus, RoleName? roleName,
    string? sortBy, bool isDescending, int page, int pageSize)
        {
            var query = _context.Accounts
                                .Include(a => a.Employee)
                                .Include(a => a.Customer)
                                .AsQueryable();

            // **Tìm kiếm theo email, họ tên, số điện thoại**
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a =>
                    a.Email.Contains(search) ||
                    a.FirstName.Contains(search) ||
                    a.LastName.Contains(search) ||
                    a.Phone.Contains(search)
                );
            }

            // **Lọc theo trạng thái tài khoản**
            if (accountStatus.HasValue)
            {
                query = query.Where(a => a.AccountStatus == accountStatus.Value);
            }

            // **Lọc theo vai trò**
            if (roleName.HasValue)
            {
                query = query.Where(a => a.RoleName == roleName.Value);
            }

            // **Sắp xếp dữ liệu**
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = isDescending
                    ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                    : query.OrderBy(e => EF.Property<object>(e, sortBy));
            }
            else
            {
                query = query.OrderByDescending(a => a.CreateAt);
            }

            int total = await query.CountAsync();

            // **Phân trang**
            var accounts = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

            return (accounts, total);
        }

        public async Task<Account> GetAccountByOrderIdAsync(Guid orderId)
        {
            return await _context
                .Orders.Where(o => o.Id == orderId)
                .Select(o => o.Account)
                .FirstOrDefaultAsync();
        }

        public async Task<Account> DeleteAsync(Guid id)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (account == null) throw new Exception("Account is not found");
            account.AccountStatus = AccountStatus.NO_ACTIVE;
            await _context.SaveChangesAsync();
            return account;
        }
    }
}
