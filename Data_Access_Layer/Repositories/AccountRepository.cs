using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
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
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Email == _email);
        }

        public async Task<Account?> GetById(Guid _id)
        {
            return await _context.Accounts
                .Include(a => a.Employee)
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == _id);
        }

        public async Task<Account> GetByPhoneNumber(string phoneNumber)
        {
            return await _context.Accounts.FirstOrDefaultAsync(a => a.Phone == phoneNumber);
        }

        public async Task<Account> UpdateAccount(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<IEnumerable<Account>> GetAllAccount()
        {
            return await _context.Accounts
                .Include(a => a.Employee) // Join v?i b?ng Employee (n?u có)
                .Include(a => a.Customer) // Join v?i b?ng Customer (n?u có)
                .ToListAsync();
        }
    }
}