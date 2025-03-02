using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Requests
{
    public class OrderUpdateRequest
    {
        public string? ReasonCancel { get; set; }
        public string RefCode { get; set; } = string.Empty;
        public OrderStatus OrderStatus { get; set; }
    }
}
