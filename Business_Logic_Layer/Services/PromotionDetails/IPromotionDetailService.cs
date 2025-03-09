using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services
{
    public interface IPromotionDetailService
    {
        Task<IEnumerable<PromotionDetailResponse>> GetAll();
        Task<PromotionDetail?> GetById(Guid id);
        Task Create(PromotionDetail promotionDetail);
        Task Update(PromotionDetail promotionDetail);
        Task Delete(Guid id);
    }
}
