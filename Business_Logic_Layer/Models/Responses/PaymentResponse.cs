using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Responses
{
    public class PaymentResponse
    {
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
        public string TransactionId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string PaymentUrl { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
