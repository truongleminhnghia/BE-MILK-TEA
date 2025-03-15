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
            //.ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.IngredientRecipes));

        }

    }
}
