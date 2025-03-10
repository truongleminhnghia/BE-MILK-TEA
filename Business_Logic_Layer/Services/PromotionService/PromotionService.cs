using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Utils;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using MailKit.Search;

namespace Business_Logic_Layer.Services.PromotionService
{
    public interface IPromotionService
    {
        Task<IEnumerable<Promotion>> GetAllPromotionAsync(
            bool isActive, string? search, string? sortBy, 
            bool isDescending, PromotionType? promotionType,
            DateTime? startDate, DateTime? endDate,
            int page, int pageSize);
        Task<PromotionResponse?> GetByIdAsync(Guid id);
        Task<PromotionResponse> CreateAsync(PromotionRequest promotion);
        //Task<Promotion?> GetByNameAsync(string name);
        Task<Promotion?> UpdateAsync(Guid id, Promotion promotion);
       
    }
    public class PromotionService : IPromotionService
    {
        private readonly IMapper _mapper;
        private readonly IPromotionRepository _promotionRepository;
        private readonly Source _source;
        public PromotionService(IMapper mapper, IPromotionRepository promotionRepository,Source source)
        {
            _mapper = mapper;
            _promotionRepository = promotionRepository;
            _source = source;
        }

        public async Task<PromotionResponse> CreateAsync(PromotionRequest promotionRequest)
        {
            try
            {
                var promotion = _mapper.Map<Promotion>(promotionRequest);
                promotion.PromotionCode = "PR" + _source.GenerateRandom8Digits();
                promotion.CreateAt = DateTime.Now;
                promotion.IsActive = true;

                var createdPromotion = await _promotionRepository.CreateAsync(promotion);

                if (createdPromotion == null)
                {
                    throw new Exception("Không thể tạo promotion. Hãy kiểm tra lại dữ liệu.");
                }

                return _mapper.Map<PromotionResponse>(createdPromotion);
            }
            catch (ArgumentException ex) 
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi hệ thống khi tạo promotion.", ex);
            }
        }

        public async Task<IEnumerable<Promotion>> GetAllPromotionAsync(bool isActive, string? search, string? sortBy, bool isDescending, PromotionType? promotionType, DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            try
            {
                return await _promotionRepository.GetAllPromotionAsync(
              isActive, search, sortBy, isDescending, promotionType, startDate, endDate, page, pageSize);

            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy list promotion", ex);
            }
        }

        public async Task<PromotionResponse?> GetByIdAsync(Guid id)
        {
            try
            {
                var promotion = await _promotionRepository.GetByIdAsync(id);
                return promotion == null ? null : _mapper.Map<PromotionResponse>(promotion);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy thông tin promotion bằng id", ex);
            }
        }

        public async Task<Promotion?> UpdateAsync(Guid id, Promotion promotion)
        {
            try
            {
                var updatedPromotion = await _promotionRepository.UpdateAsync(id, promotion);

                if (updatedPromotion == null)
                {
                    throw new Exception("Không tìm thấy promotion để cập nhật.");
                }

                return updatedPromotion;
            }
            catch (ArgumentException ex) // Lỗi ngày không hợp lệ
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật promotion.", ex);
            }
        }
    }
}
