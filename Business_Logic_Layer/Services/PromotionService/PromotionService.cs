using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Services.PromotionDetailService;
using Business_Logic_Layer.Utils;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;

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
        Task<PromotionResponse> CreateAsync(PromotionRequest promotion, double maxPriceThreshold, double minPriceThreshold);
        //Task<Promotion?> GetByNameAsync(string name);
        Task<Promotion?> UpdateAsync(Guid id, Promotion promotion, double maxPriceThreshold, double minPriceThreshold);

        Task<PageResult<PromotionResponse>> GetAllPromotions(
            bool isActive, string? search, string? sortBy, bool isDescending,
            PromotionType? promotionType, string? promotionCode, string? promotionName,
            DateTime? startDate, DateTime? endDate, int page, int pageSize);
    }
    public class PromotionService : IPromotionService
    {
        private readonly IMapper _mapper;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IPromotionDetailService _promotionDetailService;
        private readonly IPromotionDetailRepository _promotionDetailRepository;
        private readonly Source _source;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly ApplicationDbContext _context;

        public PromotionService(IMapper mapper, IPromotionRepository promotionRepository, Source source, IPromotionDetailService promotionDetailService, IPromotionDetailRepository promotionDetailRepository, IIngredientRepository ingredientRepository, ApplicationDbContext context)
        {
            _mapper = mapper;
            _promotionRepository = promotionRepository;
            _source = source;
            _promotionDetailService = promotionDetailService;
            _promotionDetailRepository = promotionDetailRepository;
            _ingredientRepository = ingredientRepository;
            _context = context;
        }

        public async Task<PromotionResponse> CreateAsync(PromotionRequest promotionRequest, double maxPriceThreshold, double minPriceThreshold)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Check validation
                if (promotionRequest.StartDate >= promotionRequest.EndDate)
                {
                    throw new ArgumentException("StartDate không thể lớn hơn hoặc bằng EndDate.");
                }
                if (promotionRequest.StartDate < DateTime.UtcNow)
                {
                    throw new ArgumentException("StartDate phải lớn hơn hoặc là thời điểm hiện tại.");
                }
                if (promotionRequest.EndDate < DateTime.UtcNow)
                {
                    throw new ArgumentException("EndDate phải lớn hơn hoặc là thời điểm hiện tại.");
                }
                if (promotionRequest.PromotionType == null)
                {
                    throw new ArgumentException("PromotionType không được để trống.");
                }

                var promotion = _mapper.Map<Promotion>(promotionRequest);
                promotion.PromotionCode = "PR" + _source.GenerateRandom8Digits();
                promotion.CreateAt = DateTime.UtcNow;
                promotion.IsActive = true;

                await _promotionRepository.CreateAsync(promotion);

                var promotionDetail = _mapper.Map<PromotionDetail>(promotionRequest.promotionDetail);
                promotionDetail.PromotionId = promotion.Id;
                if (promotionDetail.DiscountValue <= 0 || promotionDetail.DiscountValue > 100)
                {
                    throw new ArgumentException("DiscountValue không hợp lệ (phải trong khoảng 1-100%).");
                }
                await _promotionDetailService.CreateAsync(promotionDetail);

                if (promotion.PromotionType == PromotionType.PROMOTION_PRODUCT)
                {
                    // Lấy danh sách ingredient theo khoảng giá
                    // Lấy danh sách ingredient theo khoảng giá
                    var ingredients = await _ingredientRepository.GetIngredientsByPriceRangeAsync(minPriceThreshold, maxPriceThreshold);

                    // Danh sách ingredient promotion
                    var ingredientPromotions = new List<IngredientPromotion>();

                    foreach (var ingredient in ingredients)
                    {
                        // Cập nhật giá sale
                        ingredient.PricePromotion = ingredient.PriceOrigin * (1 - (promotionDetail.DiscountValue / 100));

                        // Thêm vào danh sách để lưu sau
                        ingredientPromotions.Add(new IngredientPromotion
                        {
                            PromotionId = promotion.Id,
                            IngredientId = ingredient.Id
                        });
                    }

                    // Lưu toàn bộ danh sách ingredientPromotions một lần duy nhất
                    await _promotionRepository.CreateProductPromotionsBulkAsync(ingredientPromotions);
                }
                //if (promotion.PromotionType == PromotionType.PROMOTION_ORDER) { }

                await transaction.CommitAsync();
                var result = _mapper.Map<PromotionResponse>(promotion);
                result.PromotionDetails = _mapper.Map<PromotionDetailResponse>(promotionDetail);                
                return result;
            }
            catch (ArgumentException ex)
            {
                await transaction.RollbackAsync();
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
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

        public async Task<Promotion?> UpdateAsync(Guid id, Promotion promotion, double minPriceThreshold, double maxPriceThreshold)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra Promotion có tồn tại hay không
                var existingPromotion = await _promotionRepository.GetByIdAsync(id);
                if (existingPromotion == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy promotion để cập nhật.");
                }

                // Kiểm tra tính hợp lệ của ngày tháng
                if (promotion.StartDate >= promotion.EndDate)
                {
                    throw new ArgumentException("StartDate không thể lớn hơn hoặc bằng EndDate.");
                }
                if (promotion.StartDate < DateTime.UtcNow)
                {
                    throw new ArgumentException("StartDate phải lớn hơn hoặc là thời điểm hiện tại.");
                }

                // Cập nhật dữ liệu của Promotion
                _mapper.Map(promotion, existingPromotion);
                existingPromotion.UpdateAt = DateTime.UtcNow;

                // Lưu thay đổi Promotion
                await _promotionRepository.UpdateAsync(existingPromotion.Id, existingPromotion);

                if (existingPromotion.PromotionType == PromotionType.PROMOTION_PRODUCT)
                {
                    // Lấy danh sách ingredient theo khoảng giá
                    var ingredients = await _ingredientRepository.GetIngredientsByPriceRangeAsync(minPriceThreshold, maxPriceThreshold);

                    // Cập nhật giá sale và lưu vào IngredientPromotion
                    var ingredientPromotions = new List<IngredientPromotion>();
                    foreach (var ingredient in ingredients)
                    {
                        ingredient.PricePromotion = ingredient.PriceOrigin * (1 - (existingPromotion.PromotionDetail.DiscountValue / 100));

                        ingredientPromotions.Add(new IngredientPromotion
                        {
                            PromotionId = existingPromotion.Id,
                            IngredientId = ingredient.Id
                        });
                    }

                    // Xóa danh sách cũ và thêm mới
                    await _promotionRepository.RemoveProductPromotionsByPromotionIdAsync(existingPromotion.Id);
                    await _promotionRepository.CreateProductPromotionsBulkAsync(ingredientPromotions);
                }
                await transaction.CommitAsync();
                return existingPromotion;
            }
            catch (ArgumentException ex)
            {
                await transaction.RollbackAsync();
                throw new ArgumentException(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                await transaction.RollbackAsync();
                throw new KeyNotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Lỗi khi cập nhật promotion.", ex);
            }
        }


        public async Task<PageResult<PromotionResponse>> GetAllPromotions(
        bool isActive, string? search, string? sortBy, bool isDescending,
        PromotionType? promotionType, string? promotionCode, string? promotionName,
        DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            var (promotions, total) = await _promotionRepository.GetAllPromotions(
                isActive, search, sortBy, isDescending, promotionType,
                promotionCode, promotionName, startDate, endDate, page, pageSize);

            var promotionResponses = _mapper.Map<List<PromotionResponse>>(promotions);

            return new PageResult<PromotionResponse>
            {
                Data = promotionResponses,
                PageCurrent = page,
                PageSize = pageSize,
                Total = total
            };
        }

    }
}
