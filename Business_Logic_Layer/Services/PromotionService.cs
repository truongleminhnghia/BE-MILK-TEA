using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Utils;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMapper _mapper;
        private readonly Source _source;
        private readonly ApplicationDbContext _context;

        public PromotionService(IPromotionRepository promotionRepository, IMapper mapper, Source source, ApplicationDbContext context)
        {
            _promotionRepository = promotionRepository;
            _mapper = mapper;
            _source = source;
            _context = context;

        }

        public async Task<Promotion> CreatePromotionAsync(PromotionRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var validIngredients = await _context.Ingredients
                    .Where(i => request.IngredientIds.Contains(i.Id))
                    .Select(i => i.Id)
                    .ToListAsync();

                if (validIngredients.Count != request.IngredientIds.Count)
                {
                    throw new Exception("Có nguyên liệu không tồn tại.");
                }

                if (request.StartDate <= DateTime.UtcNow)
                {
                    throw new Exception("StartDate phải lớn hơn ngày hiện tại.");
                }

                if (request.EndDate <= request.StartDate)
                {
                    throw new Exception("EndDate phải lớn hơn StartDate.");
                }

                var promotion = new Promotion
                {
                    PromotionCode = request.PromotionCode,
                    PromotionType = request.PromotionType,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    //IsActive = request.StartDate <= DateTime.UtcNow && request.EndDate >= DateTime.UtcNow
                    IsActive = false
                };
                var createdPromotion = await _promotionRepository.CreatePromotionAsync(promotion);

                var promotionDetail = new PromotionDetail
                {
                    PromotionId = createdPromotion.Id,
                    PromotionName = request.PromotionDetails.PromotionName,
                    Description = request.PromotionDetails.Description,
                    DiscountValue = request.PromotionDetails.DiscountValue,
                    MiniValue = request.PromotionDetails.MiniValue,
                    MaxValue = request.PromotionDetails.MaxValue
                };

                await _context.PromotionDetails.AddAsync(promotionDetail);

                ICollection<IngredientPromotion> promotionIngredients = new List<IngredientPromotion>();
                // Xử lý bảng trung gian
                if (request.PromotionType == PromotionType.PROMOTION_PRODUCT)
                {
                    promotionIngredients = request.IngredientIds.Select(ingredientId => new IngredientPromotion
                    {
                        PromotionId = createdPromotion.Id,
                        IngredientId = ingredientId
                    }).ToList();

                    await _context.IngredientPromotions.AddRangeAsync(promotionIngredients);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                createdPromotion.PromotionDetail = promotionDetail;
                createdPromotion.IngredientPromotions = promotionIngredients;

                return createdPromotion;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Lỗi khi tạo Promotion: " + ex.Message);
            }
        }
    }
}
