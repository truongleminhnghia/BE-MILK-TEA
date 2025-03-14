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
    public class IngredientRecipeMapper : Profile
    {
        public IngredientRecipeMapper()
        {
            CreateMap<IngredientRecipe, RecipeIngredientRequest>().ReverseMap();
            CreateMap<IngredientRecipe, RecipeIngredientResponse>().ReverseMap();
            CreateMap<RecipeIngredientRequest, RecipeIngredientResponse>().ReverseMap();
        }
    }
}
