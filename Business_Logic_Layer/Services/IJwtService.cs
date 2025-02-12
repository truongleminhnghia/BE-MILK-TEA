using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Data_Access_Layer.Repositories.Entities;

namespace Business_Logic_Layer.Services
{
    public interface IJwtService
    {
        public string GenerateJwtToken(Account _account);
        ClaimsPrincipal ValidateToken(string token);
    }
}