using Data_Access_Layer.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Requests
{
    public class PaymentCreateRequest
    {
        public Guid OrderId { get; set; }
        public string OrderDescription { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
