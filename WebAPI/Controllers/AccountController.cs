﻿using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.Utils;
using Data_Access_Layer.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        public AccountController(IAccountService accountService, IJwtService jwtService, IMapper mapper, Source source, ICustomerService customerService)
        {
            _accountService = accountService;
            _jwtService = jwtService;
            _mapper = mapper;
            _source = source;
            _customerService = customerService;
        }       

        
        [HttpGet]
        [Authorize(Roles = "ROLE_ADMIN")]
        public async Task<IActionResult> GetAllAccounts(
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool isDescending = false,
            [FromQuery] RoleName? roleName = null,
            [FromQuery] AccountStatus? accountStatus = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var accounts = await _accountService.GetAllAccounts(
                search, sortBy, isDescending, accountStatus, roleName, page, pageSize);

                var accountResponses = _mapper.Map<IEnumerable<AccountResponse>>(accounts);

                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Lấy danh sách tài khoản thành công",
                    accountResponses
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(Guid id)
        {
            try
            {
                var account = await _accountService.GetById(id);
                if (account == null)
                {
                    return NotFound(new ApiResponse(
                        HttpStatusCode.NotFound.GetHashCode(),
                        false,
                        "Không tìm thấy tài khoản",
                        null
                    ));
                }

                var accountResponse = _mapper.Map<AccountResponse>(account);
                return Ok(new ApiResponse(
                    HttpStatusCode.OK.GetHashCode(),
                    true,
                    "Lấy thông tin tài khoản thành công",
                    accountResponse
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse(HttpStatusCode.InternalServerError.GetHashCode(), false, ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountRequest request)
        {
            try
            {
                var updatedAccount = await _accountService.UpdateAccount(id, request);
                if (updatedAccount == null)
                {
                    return NotFound(new ApiResponse(
                        HttpStatusCode.NotFound.GetHashCode(),
                        false,
                        "Không tìm thấy tài khoản"
                    ));
                }

                var accountRes = _mapper.Map<AccountResponse>(updatedAccount);
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



    }
}