using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Repositories.Entities;

namespace Business_Logic_Layer.AutoMappers
{
    public class AccountMapper : Profile
    {
        public AccountMapper()
        {
            //chìu từ trái sang phải
            CreateMap<RegisterRequest, Account>();
            CreateMap<Account, AccountResponse>();
        }
    }
}