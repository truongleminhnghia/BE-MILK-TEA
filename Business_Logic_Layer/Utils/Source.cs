using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.Services;

namespace Business_Logic_Layer.Utils
{
    public class Source
    {
        /// nơi tập hợp các hàm viết chung của project
        /// 
        private readonly IJwtService _jwtService;

        public Source(IJwtService jwtService)
        {
            _jwtService = jwtService;
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

        public string GenerateRandom8Digits()
        {
            return new Random().Next(10000000, 99999999).ToString();
        }
    }
}