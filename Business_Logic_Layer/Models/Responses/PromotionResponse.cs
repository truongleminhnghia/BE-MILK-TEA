using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Responses
{
    public class PromotionResponse
    {
        public String? PromotionCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PromotionType PromotionType { get; set; }
        //public List<OrderDetailResponse> orderDetailResponses { get; set; }
    }
}
