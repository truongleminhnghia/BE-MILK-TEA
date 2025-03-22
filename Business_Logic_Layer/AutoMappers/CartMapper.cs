using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.AutoMappers
{
    public class CartMapper : Profile
    {
        public CartMapper()
        {
            CreateMap<CartResponse, Cart>().ReverseMap();
        }
    }
}