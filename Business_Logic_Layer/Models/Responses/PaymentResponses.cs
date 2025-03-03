using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
    public class PaymentResponse
    {
        public int ReturnCode { get; set; } // thành công or thất bại
        public string ReturnMessage { get; set; } = string.Empty;
        public string OrderUrl { get; set; } = string.Empty; // Link để khách thanh toán
        public string TransactionId { get; set; } = string.Empty; // ID giao dịch từ ZaloPay
    }
}
