using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.AutoMappers
{
    public class IngredientQuantityMapper : Profile
    {
        public IngredientQuantityMapper()
        {
            CreateMap<IngredientQuantityRequest, IngredientQuantity>().ReverseMap();
            CreateMap<IngredientQuantityResponse, IngredientQuantity>().ReverseMap();
            CreateMap<IngredientQuantityRequest, IngredientQuantityResponse>().ReverseMap();
        }
    }
}
