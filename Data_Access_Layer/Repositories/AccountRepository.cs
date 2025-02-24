using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
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
            _context.Accounts.Add(_account);
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

        public async Task<Account?> GetById(string _id)
        {
            return await _context.Accounts.FirstAsync(a => a.Id.Equals(_id));
        }
    }
}