using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Data_Access_Layer.Entities;
using Business_Logic_Layer.Interfaces;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/auths")]
    public class AuthenController : ControllerBase
    {
        private readonly IAuthenService _authenService;
        private readonly IAccountService _accountService;

        public AuthenController(IAuthenService authenService, IAccountService accountService)
        {
            _authenService = authenService;
            _accountService = accountService;
        }

        [HttpPost("register")]
        //public async Task<IActionResult> Register(
        //    [FromBody] RegisterRequest _request,
        //    [FromQuery] string? _typeLogin = null) // Cho phép null
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest _request)
        {
            try
            {
                var account = await _authenService.Register(_request);

                return Ok(new ApiResponse(
                    HttpStatusCode.OK,
                    true,
                    "Đăng ký thành công",
                    account
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(
                    HttpStatusCode.InternalServerError,
                    false,
                    "An error occurred during registration. Please try again."
                ));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest _request, [FromQuery] string _typeLogin)
        {
            try
            {
                if (_typeLogin == null)
                {
                    _typeLogin = TypeLogin.LOGIN_LOCAL.ToString();
                }

                var _loginSuccess = await _authenService.Login(_request, _typeLogin);
                return Ok(new ApiResponse(
                    HttpStatusCode.OK,
                    true,
                    "Đăng nhập thành công",
                    _loginSuccess
                    ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(
                    HttpStatusCode.InternalServerError,
                    false,
                    ex.Message
                ));
            }
        }

        [HttpPost("google-login")]
        public async Task<ApiResponse> GoogleLogin([FromBody] GoogleLoginModel model)
        {
            var response = await _authenService.LoginWithGoogle(model.GoogleToken);
            return response;
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _authenService.Logout();
                return Ok(new ApiResponse(
                    HttpStatusCode.OK,
                    true,
                    "Đăng xuất thành công"
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(
                    HttpStatusCode.InternalServerError,
                    false,
                    ex.Message
                ));
            }
        }

        [HttpGet]
        public async Task<string> Admin()
        {
            return await Task.FromResult("Hello");
        }


    }
}