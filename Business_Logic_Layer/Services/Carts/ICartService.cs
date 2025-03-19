using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Responses;

namespace Business_Logic_Layer.Services.Carts
{
    public interface ICartService
    {
        public Task<CartResponse> CreateCart(Guid accountId);
        public Task<CartResponse> GetByAccount(Guid accountId);
        public Task<CartResponse> GetById(Guid id);
    }
}