using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
    public class OrderResponse
    {
        String address { get; set; }
        List<OrderDetailResponse> orderDetailResponses { get; set; }
    }
}
