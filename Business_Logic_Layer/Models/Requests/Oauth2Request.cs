using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Requests
{
    public class Oauth2Request
    {
        public string FullName { get; set; } = string.Empty;
        public string GoogleAccountId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }
}