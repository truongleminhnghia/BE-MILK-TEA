using AutoMapper;
using Business_Logic_Layer.Models;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.AutoMappers
{
    public class ImageMapper : Profile
    {
        public ImageMapper()
        {
            CreateMap<Models.Image, Data_Access_Layer.Entities.Image>().ReverseMap();
        }
    }
}
