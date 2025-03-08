using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.AutoMappers;

public class OrderMapper:Profile
{
    public OrderMapper()
    {
        CreateMap<OrderRequest, Order>();
        CreateMap<OrderUpdateRequest, Order>().ReverseMap();
        CreateMap<Order, OrderResponse>();
    }
}