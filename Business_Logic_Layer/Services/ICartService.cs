using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Responses;

namespace Business_Logic_Layer.Services
{
    public interface ICartService
    {
        Task<CartResponse?> GetByIdAsync(Guid accountId);
        Task AddToCartAsync(Guid accountId, Guid ingredientProductId, int quantity);
        Task<bool> RemoveItemAsync(Guid accountId, Guid ingredientProductId);
        Task<bool> UpdateCartItemQuantityAsync(Guid accountId, Guid ingredientProductId, int quantity);
        Task<bool> ClearCartAsync(Guid accountId);

    }
}
