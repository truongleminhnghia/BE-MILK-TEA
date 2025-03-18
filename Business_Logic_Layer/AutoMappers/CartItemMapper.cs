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
            CreateMap<CartItem, CartItemResponse>()
            .ForMember(dest => dest.IngredientProductResponse, opt => opt.MapFrom(src => src.IngredientProduct));

            CreateMap<IngredientProduct, CartIngredientProductResponse>()
                .ForMember(dest => dest.Ingredient, opt => opt.MapFrom(src => src.Ingredient));

            CreateMap<Ingredient, CartIngredientResponse>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(img => img.ImageUrl).ToList()));
        }
    }
}