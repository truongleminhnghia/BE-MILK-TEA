using Business_Logic_Layer.Services.Interfaces;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Azure.Core;

namespace Business_Logic_Layer.Services
{
    public class PromotionDetailService : IPromotionDetailService
    {
        private readonly IPromotionDetailRepository _repository;

        public PromotionDetailService(IPromotionDetailRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PromotionDetailResponse>> GetAll()
        {
            var promotionDetails = await _repository.GetAll();
            return promotionDetails.Select(pd => new PromotionDetailResponse
            {
                Id = pd.Id,
                PromotionId = pd.PromotionId,
                PromotionName = pd.PromotionName, // ✅ Có trong class
                Description = pd.Description, // ✅ Có trong class
                DiscountPercent = pd.DiscountValue, // ✅ Dùng DiscountValue thay thế
                MiniValue = pd.MiniValue, // ✅ Có trong class
                MaxValue = pd.MaxValue // ✅ Có trong class
            });
        }

        public async Task<PromotionDetail?> GetById(Guid id)
        {
            return await _repository.GetById(id);
        }

        public async Task Create(PromotionDetail promotionDetail)
        {
            var newPromotionDetail = new PromotionDetail
            {
                PromotionId = promotionDetail.PromotionId,
                PromotionName = promotionDetail.PromotionName,
                Description = promotionDetail.Description,
                DiscountValue = promotionDetail.DiscountValue,
                MiniValue = promotionDetail.MiniValue,
                MaxValue = promotionDetail.MaxValue
            };

            await _repository.Create(newPromotionDetail);
        }

        public async Task Update(PromotionDetail promotionDetail)
        {
            await _repository.Update(promotionDetail);
        }

        public async Task Delete(Guid id)
        {
            await _repository.Delete(id);
        }
    }
}
