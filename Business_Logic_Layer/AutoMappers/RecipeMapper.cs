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
            CreateMap<RegisterRequest, Account>();
            CreateMap<CreateAccountRequest, Account>().ReverseMap();
            CreateMap<Account, AccountResponse>().ReverseMap();
            CreateMap<Account, MapToAccountResponse>().ReverseMap();

            CreateMap<Customer, CreateCustomerRequest>().ReverseMap();
            CreateMap<Customer, CustomerResponse>().ReverseMap();
            CreateMap<Customer, UpdateCustomerRequest>().ReverseMap();

            CreateMap<Employee, EmployeeResponse>().ReverseMap();
            CreateMap<Employee, CreateStaffRequest>().ReverseMap();
            CreateMap<Employee, UpdateStaffRequest>().ReverseMap();
        }

    }
}
