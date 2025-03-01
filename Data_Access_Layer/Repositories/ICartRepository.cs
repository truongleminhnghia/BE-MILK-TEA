using Data_Access_Layer.Entities;
using System;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public interface ICartRepository
    {
        
        Task<Cart?> GetByAccountIdAsync(Guid accountId);
        Task<Cart> Create(Cart cart);

    }
}
