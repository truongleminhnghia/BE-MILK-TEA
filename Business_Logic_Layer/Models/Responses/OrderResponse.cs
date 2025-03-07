using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Internal.Mappers;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Responses
{
    public class OrderResponse
    {
        public String? OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public String? FullNameShipping { get; set; }
        public String? PhoneShipping { get; set; }
        public String? EmailShipping { get; set; }
        public String? NoteShipping { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public double? PriceAfterPromotion { get; set; }

        public String AddressShipping { get; set; }
        public List<OrderDetailResponse> orderDetailResponses { get; set; }
        public void ConvertToOrderDetailResponse(List<OrderDetail> details, IMapper mapper)
        {
            IMapper _mapper = mapper;
            List<OrderDetailResponse> orderDetailResponse = new List<OrderDetailResponse>();
            foreach (OrderDetail orderDetail in details)
            {
                var b = _mapper.Map<OrderDetailResponse>(orderDetail);
                orderDetailResponse.Add(b);
            }
            this.orderDetailResponses = orderDetailResponse;
        }
    }
}
