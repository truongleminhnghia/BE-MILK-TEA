using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Repositories.Entities;

namespace Business_Logic_Layer.Services
{
    public interface IAccountService
    {
        Task<AccountResponse> Register(RegisterRequest _request);
    }
}