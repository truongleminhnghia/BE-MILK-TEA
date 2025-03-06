using Data_Access_Layer.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public interface ICartItemRepository
    {
        Task<IEnumerable<CartItem>> GetAll();
        Task<CartItem> GetById(Guid id);
        Task Add(CartItem cartItem);
        Task<CartItem> Update(CartItem cartItem);
        Task<bool> Delete(Guid id);
        Task<CartItem> Create(CartItem cartItem);

    }
}
