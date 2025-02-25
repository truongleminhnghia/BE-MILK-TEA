using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.AutoMappers
{
    public class IngredientProductMapper : Profile
    {
       public IngredientProductMapper() 
        {
            CreateMap<IngredientProduct, IngredientProductRequest>().ReverseMap();
        }
    }
}
