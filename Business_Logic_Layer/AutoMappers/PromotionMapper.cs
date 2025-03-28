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
    public class PromotionMapper:Profile
    {
        public PromotionMapper()
        {
            CreateMap<PromotionRequest, Promotion>().ReverseMap();
            CreateMap<Promotion, PromotionResponse>().ReverseMap();
            CreateMap<PromotionUpdateRequest, Promotion>().ReverseMap();
            CreateMap<Promotion, PromotionResponse>()
                .ForMember(dest => dest.PromotionDetails, opt => opt.MapFrom(src => src.PromotionDetail));
            CreateMap<ActivePromotionResponse, Promotion>().ReverseMap();
        }
    }
}
