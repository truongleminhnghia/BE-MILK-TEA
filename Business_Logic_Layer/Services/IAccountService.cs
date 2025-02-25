using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.Interfaces
{
    public interface IAccountService
    {
        public Task<Account?> GetById(Guid _id);
        public Task<Account?> GetByEmail(string _email);
        Task<Account> CreateAccount(CreateAccountRequest createAccountRequest);



    }
}