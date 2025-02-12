using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;

namespace Business_Logic_Layer.Services
{
    public interface IAuthenService
    {
        Task<AccountResponse> Register(RegisterRequest _request, string _type);
        Task<string> Login(LoginRequest _request, string _type);       
        Task<string> GenerateUrl(string _type);
    }
}