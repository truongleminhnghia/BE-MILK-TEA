using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public interface IPromotionDetailRepository
    {
        Task<IEnumerable<PromotionDetail>> GetAll();
        Task<PromotionDetail?> GetById(Guid id);
        Task Create(PromotionDetail promotionDetail);
        Task Update(PromotionDetail promotionDetail);
        Task Delete(Guid id);
    }
}
