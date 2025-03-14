using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using MailKit.Search;

namespace Business_Logic_Layer.Services.PromotionDetailService
{
    public class PromotionDetailService : IPromotionDetailService
    {
        private readonly IPromotionDetailRepository _promotionDetailRepository;
        private readonly IMapper _mapper;
        public PromotionDetailService(IPromotionDetailRepository promotionDetailRepository,IMapper mapper)
        {
            _promotionDetailRepository = promotionDetailRepository;
            _mapper = mapper;
        }

        public async Task<PromotionDetail> CreateAsync(PromotionDetail promotionDetail)
        {
            try
            {
                if (promotionDetail == null)
                {
                    throw new ArgumentNullException(nameof(promotionDetail), "không có thông tin promotionDetail ");
                }

                return await _promotionDetailRepository.CreateAsync(promotionDetail);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể tạo chi tiết đơn hàng", ex);
            }
        }

        public async Task<bool> DeleteByIdAsync(Guid id)
        {
            try
            {
                return await _promotionDetailRepository.DeleteByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể xóa promotion detail với ID: {id}", ex);
            }
        }

        public async Task<List<PromotionDetailResponse>> GetAllPromotionDetailAsync(Guid Id)
        {
            try
            {
                var promotionDetails = await _promotionDetailRepository.GetAllPromotionDetailAsync(Id);
                return _mapper.Map<List<PromotionDetailResponse>>(promotionDetails);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy danh sách chi tiết đơn hàng", ex);
            }
        }

        public async Task<PromotionDetail> GetbyId(Guid id)
        {
            try
            {
                return await _promotionDetailRepository.GetbyId(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể lấy promotion detail với ID: {id}", ex);
            }
        }

        public async Task<PromotionDetail> UpdateAsync(Guid id, PromotionDetail promotionDetail)
        {
            try
            {
                return await _promotionDetailRepository.UpdateAsync(id, promotionDetail);
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể cập nhật promotion detail với ID: {id}", ex);
            }
        }
    }
}
