using Data_Access_Layer.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public string TaxCode { get; set; }
        public string Address { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AccountLevelEnum AccountLevel { get; set; }
    }
}
