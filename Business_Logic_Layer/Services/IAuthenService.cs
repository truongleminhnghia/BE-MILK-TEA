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
        Task<AccountResponse> Register(CreateAccountRequest _request);
        Task<AuthenticateResponse> LoginLocal(LoginRequest _request, string _type);

        public Task<Dictionary<string, object>> AuthenticateAndFetchProfile(string code, string type);

        public string GenerateUrl(string type);
        public Task<AuthenticateResponse> LoginOauth2(Oauth2Request _request);
    }
}