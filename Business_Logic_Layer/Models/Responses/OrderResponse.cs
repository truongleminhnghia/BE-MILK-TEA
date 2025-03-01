using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Responses
{
    public class OrderResponse
    {
        String? OrderCode { get; set; }
        DateTime OrderDate { get; set; }
        String? FullNameShipping { get; set; }
        String? PhoneShipping { get; set; }
        String? EmailShipping { get; set; }
        String? NoteShipping { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public double? PriceAfterPromotion { get; set; }
        public String RefCode { get; set; } = string.Empty;
        public String? ReasonCancel { get; set; }

        String AddressShipping { get; set; }
     //   List<OrderDetailResponse> orderDetailResponses { get; set; }
        
    }
}
