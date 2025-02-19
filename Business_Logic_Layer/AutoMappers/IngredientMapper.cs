using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models;

namespace Business_Logic_Layer.AutoMappers
{
    public class IngredientMapper : Profile
    {
        public IngredientMapper()
        {
            CreateMap<Ingredient, Data_Access_Layer.Entities.Ingredient>().ReverseMap();
        }
    }
}
