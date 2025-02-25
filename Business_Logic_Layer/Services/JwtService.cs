using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Business_Logic_Layer.Services
{
    public class JwtService : IJwtService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JwtService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal GetUserClaims()
        {
            return _httpContextAccessor.HttpContext?.User;
        }
        public string GenerateJwtToken(Account _account)
        {
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
            var expiryMinutes = Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES");

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) ||
                string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(expiryMinutes))
            {
                throw new InvalidOperationException("JWT environment variables are not set properly.");
            }
            var _tokeId = Guid.NewGuid();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("aud", audience),
                    new Claim("iss", issuer),
                    new Claim("accountId", _account.Id.ToString()),
                    new Claim("email", _account.Email),
                    new Claim("roleName", _account.RoleName.ToString()),
                    new Claim("tokenId", _tokeId.ToString())
                }),

                Expires = DateTime.UtcNow.AddDays(7), // token hết hạng trong 7 ngày
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GetAccountId()
        {
            return GetUserClaims().FindFirst("accountId")?.Value;
        }

        public string GetEmail()
        {
            return GetUserClaims().FindFirst("email")?.Value;
        }

        public string GetRole()
        {
            return GetUserClaims().FindFirst("roleName")?.Value; ;
        }

        public string GetTokenId()
        {
            return GetUserClaims().FindFirst("tokenId")?.Value;
        }

        public int? ValidateToken(string token)
        {
            if (token == null)
                return null;
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT environment variables are not set properly.");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}