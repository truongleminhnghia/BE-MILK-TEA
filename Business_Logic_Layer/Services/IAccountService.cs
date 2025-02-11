using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Data_Access_Layer.Repositories.Entities;

namespace Business_Logic_Layer.Services
{
    public interface IAccountService
    {
        Task<Account> Register(RegisterRequest _request);
    }
}