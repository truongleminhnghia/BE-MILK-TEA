using AutoMapper;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.AutoMappers
{
    public class ImageMapper : Profile
    {
        public ImageMapper()
        {
            CreateMap<ImageRequest, Image>().ReverseMap();
            CreateMap<ImageRespone, Image>().ReverseMap();
            CreateMap<ImageRequest, ImageRespone>().ReverseMap();
        }
    }
}
