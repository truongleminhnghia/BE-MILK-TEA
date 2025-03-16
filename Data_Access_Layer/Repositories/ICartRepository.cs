using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Repositories
{
    public interface ICartRepository
    {
        Task<Cart> GetOrCreateCartAsync(Guid accountId);
        Task<Cart> CreateAsync(Cart cart);
        Task UpdateCartItemQuantityAsync(Guid accountId, Guid ingredientProductId, int quantity);
        Task ClearCartAsync(Guid accountId);
        Task<Cart?> GetByAccountAsync(Guid id);
    }
}