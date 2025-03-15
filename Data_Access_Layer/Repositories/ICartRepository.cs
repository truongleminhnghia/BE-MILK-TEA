using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetByIdAsync(Guid id);
        Task<Cart> CreateAsync(Cart cart);
        Task<Cart?> UpdateAsync(Guid id, Cart cart);
        Task<Cart?> GetByAccountAsync(Guid id);

    }
}