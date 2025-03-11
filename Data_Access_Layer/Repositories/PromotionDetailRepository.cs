using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Repositories
{
    public class PromotionDetailRepository : IPromotionDetailRepository
    {
        public Task<PromotionDetail> CreateAsync(PromotionDetail promotionDetail)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<PromotionDetail>> GetAllPromotionDetailAsync(Guid PromotionId)
        {
            throw new NotImplementedException();
        }

        public Task<PromotionDetail> GetbyId(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<PromotionDetail> UpdateAsync(Guid id, PromotionDetail promotionDetail)
        {
            throw new NotImplementedException();
        }
    }
}
