using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;

namespace Business_Logic_Layer.Services.Carts
{
    public interface ICartItemService
    {
        Task<CartItemResponse> CreateCartItem(CartItemRequest cartItemRequest);
        Task<CartItemResponse> GetById(Guid id);
        Task<IEnumerable<CartItemResponse>> GetByCart(Guid cartId);
        Task<bool> UpdateCartItem(Guid id, UpdateCartItemRequest request);
        Task<bool> Delete(Guid id);
    }
}