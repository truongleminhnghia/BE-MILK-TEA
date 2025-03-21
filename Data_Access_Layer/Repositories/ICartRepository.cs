using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Repositories
{
    public interface ICartRepository
    {
        Task<Cart> Save(Cart cart);
        Task<Cart> FindById(Guid id);
        Task<Cart> FindByAccount(Guid id);
    
    }
}