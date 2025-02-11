using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Repositories.Data;
using Data_Access_Layer.Repositories.Entities;
using Data_Access_Layer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer.Repositories.Implements
{
    public class AccountRepository : IAccountRepository
    {

        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Account> Create(Account _account)
        {
            return Task.FromResult(_account);
        }

        public Task<Account> GetByEmail(string _email)
        {
            throw new NotImplementedException();
        }

        public Task<Account> GetById(string _id)
        {
            throw new NotImplementedException();
        }
    }
}