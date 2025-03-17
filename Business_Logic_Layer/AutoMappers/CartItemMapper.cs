using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Models.Requests;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Business_Logic_Layer.AutoMappers
{
    public class CartItemMapper : Profile
    {
        public CartItemMapper()
        {
            CreateMap<CartItemRequest, CartItem>().ReverseMap();
            CreateMap<CartItem, CartItemResponse>().ReverseMap();
        }
    }
}