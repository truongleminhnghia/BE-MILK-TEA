using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
    public class AuthenticateResponse
    {
        public string Token { get; set; } = string.Empty;
        public AccountResponse AccountResponse { get; set; } = new AccountResponse();

        public AuthenticateResponse(string _token, AccountResponse _accountResponse)
        {
            Token = _token;
            AccountResponse = _accountResponse;
        }
    }
}