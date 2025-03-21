using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.AutoMappers
{
    public class CartItemMapper : Profile
    {
        public CartItemMapper()
        {
            CreateMap<CartItem, CartItemRequest>().ReverseMap();
            CreateMap<CartItem, CartItemResponse>()
            .ForMember(dest => dest.Ingredient, opt => opt.MapFrom(src => src.Ingredient))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Ingredient.PriceOrigin))  // ✅ Assign Price from Ingredient
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.Ingredient.PriceOrigin));  // ✅ Calculate Total Price

            CreateMap<Ingredient, IngredientResponse>() // Ensure IngredientResponse is mapped
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images)) // Map images list
                .ForMember(dest => dest.IngredientQuantities, opt => opt.MapFrom(src => src.IngredientQuantities));
            CreateMap<Image, ImageResponse>(); // Ensure Image mapping
            CreateMap<IngredientQuantity, IngredientQuantityResponse>();

        }
        
    }
}