using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Business_Logic_Layer.Models.Requests
{
    public class ZaloPayCallback
    {
        [JsonProperty("app_trans_id")]
        public string OrderId { get; set; } = string.Empty;

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; } // thành công or thất bại

        [JsonProperty("zp_trans_id")]
        public string TransactionId { get; set; } = string.Empty;
    }
}
