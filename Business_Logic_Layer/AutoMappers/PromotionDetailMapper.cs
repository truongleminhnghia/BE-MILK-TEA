using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.AutoMappers
{
    public class PromotionDetailMapper : Profile
    {
        public PromotionDetailMapper() 
        {
            CreateMap<PromotionDetailRequest, PromotionDetail>().ReverseMap();
            CreateMap<PromotionDetail, PromotionDetailResponse>().ReverseMap();
            CreateMap<PromotionDetailUpdateRequest, PromotionDetail>().ReverseMap();
            CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.orderDetailResponses, opt => opt.MapFrom(src => src.OrderDetails));

        }

    }
}
