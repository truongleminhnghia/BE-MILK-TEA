using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Data_Access_Layer.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/auths")]
    [AllowAnonymous]
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
        public async Task<IActionResult> Register([FromBody] CreateAccountRequest _request) // Cho phép null
        {
            try
             {
                var account = await _authenService.Register(_request, false);

                return Ok(new ApiResponse (HttpStatusCode.OK.GetHashCode(), true, "Đăng ký thành công", account));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse( HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest? request, [FromQuery] string typeLogin)
        {
            try
            {
                if (typeLogin.Equals(TypeLogin.LOGIN_LOCAL.ToString()))
                {
                    var loginSuccess = await _authenService.LoginLocal(request, typeLogin);
                    return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Đăng nhập thành công", loginSuccess));
                }
                else if (typeLogin.Equals(TypeLogin.LOGIN_GOOGLE.ToString()))
                {
                    var urlLogin = _authenService.GenerateUrl(TypeLogin.LOGIN_GOOGLE.ToString());
                    return Ok(new ApiResponse(
                        HttpStatusCode.OK.GetHashCode(),
                        true,
                        "Create URL successfull",
                        urlLogin
                        ));
                }
                return BadRequest(new ApiResponse (HttpStatusCode.BadRequest.GetHashCode(), false, "Đăng nhập thất bại" ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse (HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }


        [HttpGet("callback")]
        public async Task<IActionResult> CallbackAuthenticate([FromQuery] string code, [FromQuery] string type_login)
        {
            try
            {
                var infoUser = await _authenService.AuthenticateAndFetchProfile(code, type_login);
                if (infoUser == null)
                {
                    return BadRequest(new ApiResponse(HttpStatusCode.BadRequest.GetHashCode(), false, "failed", null));
                }
                if (type_login.Equals(TypeLogin.LOGIN_GOOGLE.ToString()))
                {
                    var oauth2 = new Oauth2Request
                    {
                        FullName = infoUser.ContainsKey("name") ? infoUser["name"].ToString() : "",
                        GoogleAccountId = infoUser.ContainsKey("sub") ? infoUser["sub"].ToString() : "",
                        Email = infoUser.ContainsKey("email") ? infoUser["email"].ToString() : "",
                        Avatar = infoUser.ContainsKey("picture") ? infoUser["picture"].ToString() : "",
                        PhoneNumber = "",
                    };
                    var result = await _authenService.LoginOauth2(oauth2);
                    return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "success", result));
                }
                return BadRequest(new ApiResponse(HttpStatusCode.BadRequest.GetHashCode(), false, "failed", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message, null));
            }

        }

    }
}