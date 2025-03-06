using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;

namespace Business_Logic_Layer.Services.Cart
{
    public interface ICartService
    {
        Task<IEnumerable<CartService>> GetAll();
        Task<CartService> GetById(Guid id);
        Task<CartService> Create(CartService cart);
        Task<CartService> Update(CartService cart);
        Task<bool> Delete(Guid id);
        Task<bool> CreateCart(CartRequest request);
    }
}
