using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.Services
{
    public interface ICartService
    {
        public Task<CartResponse?> GetByIdAsync(Guid id);
        public Task<CartResponse?> GetByAccountAsync(Guid id);
        public Task<CartResponse> CreateAsync(CartRequest request);
        public Task<bool?> UpdateAsync(Guid id, CartRequest request);
    }
}