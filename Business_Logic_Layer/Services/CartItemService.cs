using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services
{
    public interface ICartItemService
    {
        Task<CartItem?> GetByIdAsync(Guid id);
        Task<CartItem> AddToCartAsync(CartItem cartItem);
        Task<bool> RemoveCartItemByIdAsync(Guid id);

    }
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;
        
        public CartItemService(ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }

        public async Task<CartItem> AddToCartAsync(CartItem cartItem)
        {
            return await _cartItemRepository.AddToCartAsync(cartItem);
        }

        public async Task<bool> RemoveCartItemByIdAsync(Guid id)
        {
            return await _cartItemRepository.RemoveCartItemByIdAsync(id);
        }

        public async Task<CartItem?> GetByIdAsync(Guid id)
        {
            return await _cartItemRepository.GetByIdAsync(id);
        }


    }
}
