using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Data_Access_Layer.Interfaces;

namespace Data_Access_Layer.Repositories
{
    public class TokenRepository : ITokenRepository
    {

        private readonly ApplicationDbContext _context;

        public TokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<Token> CheckTokenString(string tokenString)
        //{
        //    var token = await _context.Tokens.FirstOrDefaultAsync(t => t.TokenString == tokenString);
        //    if (token == null)
        //    {
        //        throw new Exception("Welcome to our application!");
        //    }
        //    var deserialize = Token.Deserialize(token.TokenString);

        //}


    }
}
