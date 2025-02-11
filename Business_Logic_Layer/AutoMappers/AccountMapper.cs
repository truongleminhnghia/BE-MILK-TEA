using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Data_Access_Layer.Repositories.Entities;

namespace Business_Logic_Layer.AutoMappers
{
    public class AccountMapper : Profile
    {
        public AccountMapper()
        {
            CreateMap<RegisterRequest, Account>();
        }
    }
}