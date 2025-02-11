using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Repositories.Entities;

namespace Data_Access_Layer.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> Create(Account _account);
        Task<Account> GetById(string _id);
        Task<Account> GetByEmail(string _email);
    }
}