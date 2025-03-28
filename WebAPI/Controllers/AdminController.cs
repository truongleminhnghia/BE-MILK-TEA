using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Utils;
using Data_Access_Layer.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/admins")]
    [Authorize(Roles = "ROLE_ADMIN")]
    public class AdminController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly Source _source;
        private readonly IAuthenService _authenService;

        public AdminController(IAccountService accountService, IJwtService jwtService, IMapper mapper, Source source, IEmployeeService employeeService, IAuthenService authenService)
        {
            _accountService = accountService;
            _jwtService = jwtService;
            _mapper = mapper;
            _source = source;
            _employeeService = employeeService;
            _authenService = authenService;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(Guid id)
        {
            try
            {
                var account = await _accountService.GetById(id);
                if (account == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(
                        HttpStatusCode.InternalServerError.GetHashCode(),
                        false,
                        "Không tìm thấy tài khoản"
                    ));                    
                }
                return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Lấy tài khoản thành công", account));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }

        [HttpPost("create-account")]
        public async Task<IActionResult> Register([FromBody] CreateAccountRequest _request) // Cho phép null
        {
            try
            {
                var account = await _authenService.Register(_request, true);

                return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Đăng ký thành công", account));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            try
            {
                var account = await _accountService.DeleteAccount(id);
                return Ok(new ApiResponse(HttpStatusCode.OK.GetHashCode(), true, "Cấm tài khoản thành công", account));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }

        [HttpGet]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> GetAllAccounts(
            [FromQuery] string? search = null,
            [FromQuery] AccountStatus? accountStatus = null,
            [FromQuery] RoleName? roleName = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool isDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var accounts = await _accountService.GetAllAccountsAsync(
                            search, accountStatus, roleName, sortBy, isDescending, page, pageSize);
                if (accounts == null)
                {
                    return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest.GetHashCode(),
                    true,
                    "Danh sách rỗng",
                    accounts
                ));
                }
                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Lấy danh sách tài khoản thành công",
                    accounts
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message, null));
            }
        }
    }
}
