using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Utils;
using Data_Access_Layer.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using XAct.Messages;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/accounts")]

    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ICustomerService _customerService;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly Source _source;
        private readonly IRedisService _redisCacheService;
        private const string AccountsCachePrefix = "accounts_cache";

        public AccountController(IAccountService accountService, IJwtService jwtService, IMapper mapper, Source source, ICustomerService customerService, IRedisService redisCacheService)
        {
            _accountService = accountService;
            _jwtService = jwtService;
            _mapper = mapper;
            _source = source;
            _customerService = customerService;
            _redisCacheService = redisCacheService;
        }


        [HttpGet]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_STAFF, ROLE_MANAGER")]
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
                var cacheKey = $"{AccountsCachePrefix}:{search}:{accountStatus}:{roleName}:{sortBy}:{isDescending}:{page}:{pageSize}";
                // Try to get from cache first
                var cachedAccounts = await _redisCacheService.GetAsync<PagedResponse<AccountResponse>>(cacheKey);
                if (cachedAccounts != null)
                {
                    return Ok(new ApiResponse(
                        HttpStatusCode.OK.GetHashCode(),
                        true,
                        "Lấy danh sách tài khoản thành công (from cache)",
                        cachedAccounts
                    ));
                }

                var accounts = await _accountService.GetAllAccountsAsync(
                            search, accountStatus, roleName, sortBy, isDescending, page, pageSize);
                if(accounts == null) {
                    return BadRequest(new ApiResponse(
                    HttpStatusCode.BadRequest.GetHashCode(),
                    true,
                    "Danh sách rỗng",
                    accounts
                ));
                }
                await _redisCacheService.SetAsync(cacheKey, accounts, TimeSpan.FromMinutes(10));

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

        

        [HttpPut("{id}")]
        [Authorize(Roles = "ROLE_ADMIN, ROLE_CUSTOMER)")]
        public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountRequest request)
        {
            try
            {
                
                var updatedAccount = await _accountService.UpdateAccount(id, request);
                if (updatedAccount == null)
                {
                    return BadRequest(new ApiResponse(
                        HttpStatusCode.BadRequest.GetHashCode(),
                        true,
                        "Không tìm thấy tài khoản"
                    ));
                }

                var accountRes = _mapper.Map<AccountResponse>(updatedAccount);
                // Invalidate cache for this account
                await _redisCacheService.RemoveAsync($"{AccountsCachePrefix}:{id}");

                // Invalidate all accounts list cache
                await _redisCacheService.RemoveByPrefixAsync(AccountsCachePrefix);
                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Cập nhật thành công",
                    accountRes
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentAccount()
        {
            try
            {
                var account = await _accountService.GetCurrentAccount();
                if (account == null)
                {
                    return BadRequest(new ApiResponse(
                        HttpStatusCode.BadRequest.GetHashCode(),
                        true,
                        "Không tìm thấy tài khoản"
                    ));
                }
                var accountRes = _mapper.Map<AccountResponse>(account);
                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Lấy thông tin tài khoản thành công",
                    accountRes
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, "Lỗi:" + ex.Message));
            }
        }

    }
}