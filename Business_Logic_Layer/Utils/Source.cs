using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Data_Access_Layer.Repositories;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Business_Logic_Layer.Services;

namespace Business_Logic_Layer.Utils
{
    public class Source
    {
        /// nơi tập hợp các hàm viết chung của project
        /// 
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        public Source(IJwtService jwtService, IHttpContextAccessor httpContextAccessor, IAccountRepository accountRepository, IMapper mapper)
        {
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public string CheckRoleName()
        {
            var roleName = _jwtService.GetRole();

            switch (roleName)
            {
                case "ROLE_ADMIN":
                    return "ADMIN";
                case "ROLE_STAFF":
                    return "STAFF";
                case "ROLE_MANAGER":
                    return "MANAGER";
                case "ROLE_CUSTOMER":
                    return "CUSTOMER";
                default:
                    return "NO_ROLE";
            }
        }

        public bool CheckAccountId(string _id)
        {
            bool result;
            var accountCurrent = _jwtService.GetAccountId();
            if (!_id.Equals(accountCurrent))
            {
                result = false;
            }
            result = true;
            return result;
        }

        // ham nay dung de lay thong tin cua user dang dang nhap
        public async Task<Account?> GetCurrentAccount()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Người dùng chưa đăng nhập hoặc token không hợp lệ"); ;
            }

            return await _accountRepository.GetById(userId);
        }

        public string GenerateRandom8Digits()
        {
            return new Random().Next(10000000, 99999999).ToString();
        }

        public int CheckDate(DateTime input)
        {
            // Lấy ngày hiện tại
            DateTime today = DateTime.Now;
            int daysRemaining = (input - today).Days;
            if (daysRemaining < 0)
            {
                return -1; // hết date
            }
            else if (daysRemaining <= 10)
            {
                return 1; // còn dưới 10 ngày
            }
            else
            {
                return 0;
            }
        }
    }
}