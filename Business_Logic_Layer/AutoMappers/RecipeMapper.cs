using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.AutoMappers
{
    public class RecipeMapper : Profile
    {

        public RecipeMapper()
        {
            //chìu từ trái sang phải
            CreateMap<Recipe, RecipeRequest>().ReverseMap();
            CreateMap<Recipe, RecipeResponse>().ReverseMap();
            CreateMap<RecipeRequest, RecipeResponse>().ReverseMap();
            CreateMap<Recipe, RecipeResponse>()
            .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdateAt));

            CreateMap<Category, CategoryResponse>()
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt ?? DateTime.MinValue));

            CreateMap<IngredientRecipe, RecipeIngredientResponse>();

            CreateMap<Ingredient, IngredientResponse>()
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => src.CreateAt ?? DateTime.MinValue))
                .ForMember(dest => dest.UpdateAt, opt => opt.MapFrom(src => src.UpdateAt ?? DateTime.MinValue));

        }

    }
}
