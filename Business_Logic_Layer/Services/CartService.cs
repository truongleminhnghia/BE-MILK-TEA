using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Org.BouncyCastle.Asn1.Cmp;

namespace Business_Logic_Layer.Services
{
    public interface ICartService
    {
        public Task<Cart?> GetByIdAsync(Guid id);
        public Task<CartResponse> CreateAsync(CartRequest cart);
        public Task<Cart?> UpdateAsync(Guid id, Cart cart);
    }
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public Task<CartResponse> CreateAsync(CartRequest cart)
        {
            throw new NotImplementedException();
        }

        public async Task<Cart?> GetByIdAsync(Guid id)
        {
            return await _cartRepository.GetByIdAsync(id);
        }

        public async Task<Cart?> UpdateAsync(Guid id, Cart cart)
        {
            return await _cartRepository.UpdateAsync(id, cart);
        }
    }
}
