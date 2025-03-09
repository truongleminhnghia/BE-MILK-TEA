using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Responses
{
    public class AccountResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccountStatus AccountStatus { get; set; }
        public string Phone { get; set; } = string.Empty;

        public string ImageUrl { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RoleName RoleName { get; set; }

        public EmployeeResponse? Employee { get; set; }
        public CustomerResponse? Customer { get; set; }

    }
}