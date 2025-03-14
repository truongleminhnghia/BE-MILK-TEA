using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Configurations
{
    public class VNPayConfiguration
    {
        public string Version { get; set; } = "2.1.0";
        public string TmnCode { get; set; } // Terminal ID provided by VNPay
        public string HashSecret { get; set; } // Secret key for creating signature
        public string PaymentUrl { get; set; } // VNPay payment URL
        public string ReturnUrl { get; set; } // Your callback URL
        public string OrderInfo { get; set; } // Default order info
        public string Command { get; set; } = "pay";
        public string CurrCode { get; set; } = "VND";
        public string Locale { get; set; } = "vn";
    }
}
