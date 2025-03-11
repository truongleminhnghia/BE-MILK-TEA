using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services.PromotionDetailService
{
    public class PromotionDetailService : IPromotionDetailService
    {
        private readonly IPromotionDetailRepository _promotionDetailRepository; 

        public Task<PromotionDetail> CreateAsync(PromotionDetail promotionDetail)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<PromotionDetailResponse>> GetAllPromotionDetailAsync(Guid PromotionId)
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
