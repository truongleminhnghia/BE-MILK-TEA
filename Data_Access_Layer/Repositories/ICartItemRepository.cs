using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Repositories
{
    public interface ICartItemRepository
    {
        Task<CartItem> SaveCartItem(CartItem cartItem);
        Task<IEnumerable<CartItem>> GetByCart(Guid cartId);
        Task<CartItem> GetById(Guid id);
        Task<bool> UpdateCartItem(CartItem cartItem);
        Task<bool> DeleteCartItem(CartItem cartItem);
    }
}