using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.Services.PromotionDetailService
{
    public interface IPromotionDetailService
    {
        public Task<PromotionDetail> CreateAsync(PromotionDetail promotionDetail);
        public Task<List<PromotionDetailResponse>> GetAllPromotionDetailAsync(Guid PromotionId);
        public Task<PromotionDetail> GetbyId(Guid id);
        public Task<PromotionDetail> UpdateAsync(Guid id, PromotionDetail promotionDetail);
        public Task<bool> DeleteByIdAsync(Guid id);
    }
}
