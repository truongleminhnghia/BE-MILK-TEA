using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Repositories
{
    public interface IPromotionDetailRepository
    {
        public Task<PromotionDetail> CreateAsync(PromotionDetail promotionDetail);
        public Task<List<PromotionDetail>> GetAllPromotionDetailAsync(Guid PromotionId);
        public Task<PromotionDetail> GetbyId(Guid id);
        public Task<PromotionDetail> UpdateAsync(Guid id, PromotionDetail promotionDetail);
        public Task<bool> DeleteByIdAsync(Guid id);
    }
}
