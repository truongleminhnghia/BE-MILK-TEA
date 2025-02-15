using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Business_Logic_Layer.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                AttachUserToContext(context, token);
            }

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
                var _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
                var _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
                if (string.IsNullOrEmpty(_secretKey) || string.IsNullOrEmpty(_issuer) || string.IsNullOrEmpty(_audience))
                {
                    throw new InvalidOperationException("JWT SECRET KEY environment variables is null");
                }
                var _key = Encoding.UTF8.GetBytes(_secretKey);
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Không cho phép thời gian trễ giữa các req
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                if (validatedToken is not JwtSecurityToken jwtToken) throw new SecurityTokenException("Invalid JWT token");

                // lưu thông tin người dùng vào HttpConext
                context.Items["User"] = principal;
            }
            catch (SecurityTokenExpiredException)
            {
                Console.WriteLine("JWT token has expired");
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine($"JWt token validtion failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in JWT Middleware: {ex.Message}");
            }
        }
    }
}