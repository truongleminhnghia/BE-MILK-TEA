using System.Collections.Generic;
using System.Threading.Tasks;
using Business_Logic_Layer.Services.Interfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;

        public CartItemService(ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }

        public async Task<IEnumerable<CartItem>> GetAll()
        {
            return await _cartItemRepository.GetAll();
        }

        public async Task<CartItem> GetById(Guid id)
        {
            return await _cartItemRepository.GetById(id);
        }

        public async Task<CartItem> Create(CartItem cartItem)
        {
            return await _cartItemRepository.Create(cartItem);
        }

        public async Task<CartItem> Update(CartItem cartItem)
        {
            return await _cartItemRepository.Update(cartItem);
        }

        public async Task<bool> Delete(Guid id)
        {
            return await _cartItemRepository.Delete(id);
        }
    }
}
