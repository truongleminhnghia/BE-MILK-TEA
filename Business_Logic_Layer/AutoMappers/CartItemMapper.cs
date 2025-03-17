using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.DTO;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
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

            CreateMap<Cart, CartResponse>()
    .ForMember(dest => dest.CartItemResponse, opt => opt.MapFrom(src => src.CartItems));
            CreateMap<CartItem, CartItemResponse>()
    .ForMember(dest => dest.IngredientProductResponse, opt => opt.MapFrom(src => src.IngredientProduct));

        }
    }
}