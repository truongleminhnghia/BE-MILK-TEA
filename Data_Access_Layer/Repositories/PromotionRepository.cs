
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Data_Access_Layer.Enum;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer.Repositories
{
    public interface IPromotionRepository
    {
        Task<IEnumerable<Promotion>> GetAllPromotionAsync(
            bool? IsActive, string? search, string? sortBy,
            bool isDescending, PromotionType? promotionType,
            DateTime? startDate, DateTime? endDate,
            int page, int pageSize);

        Task<(List<Promotion>, int)> GetAllPromotions(
        bool isActive, string? search, string? sortBy, bool isDescending,
        PromotionType? promotionType, string? promotionCode, string? promotionName,
        DateTime? startDate, DateTime? endDate, int page, int pageSize);
        Task<Promotion?> GetByIdAsync(Guid id);
        Task<Promotion?> GetByCodeAsync(String code);
        Task<Promotion> CreateAsync(Promotion promotion);
        Task<Promotion?> UpdateAsync(Guid id, Promotion promotion);
        Task<IngredientPromotion> CreateProductPromotion(IngredientPromotion ingredientPromotion);
        Task<OrderPromotion> CreateOrderPromotion(OrderPromotion orderPromotion);
        //Task<bool> DeleteAsync(Guid id);
        Task CreateProductPromotionsBulkAsync(List<IngredientPromotion> ingredientPromotions);
        Task RemoveProductPromotionsByPromotionIdAsync(Guid promotionId);

        Task<List<Promotion>> GetFilteredPromotionsAsync(
    string? search, string? sortBy, bool isDescending,
    PromotionType? promotionType, string? promotionName,
    DateOnly? startDate, DateOnly? endDate, bool? isActive);
        Task<Promotion> DeleteAsync(Guid id);
        Task<List<Promotion>> GetActivePromotions(PromotionType? promotionType, double? orderTotalPrice, DateOnly? expiredDate, bool? isActive);
        Task<Promotion?> GetByIdAndCode(Guid? id, string? code);
    }
}

namespace Data_Access_Layer.Repositories
{

    public class PromotionRepository : IPromotionRepository
    {
        private readonly ApplicationDbContext _context;

        public PromotionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Promotion> CreateAsync(Promotion promotion)
        {
            try
            {
                // Lưu Promotion
                promotion.CreateAt = DateTime.UtcNow;
                await _context.Promotions.AddAsync(promotion);
                await _context.SaveChangesAsync();
                return promotion;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo promotion.", ex);
            }
        }

        public async Task<IEnumerable<Promotion>> GetAllPromotionAsync(
            bool? IsActive, string? search, string? sortBy, bool isDescending,
            PromotionType? promotionType, DateTime? startDate, DateTime? endDate,
            int page, int pageSize)
        {
            var query = _context.Promotions.AsQueryable();

            // **Filtering by name**
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.PromotionCode.Contains(search));
            }
            if (IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == IsActive.Value);
            }
            if (promotionType.HasValue)
            {
                query = query.Where(p => p.PromotionType == promotionType.Value);
            }

            // **Filtering by PromotionType**
            if (promotionType.HasValue)
            {
                query = query.Where(p => p.PromotionType == promotionType.Value);
            }

            // **Filtering by date range (StartDate - EndDate)**
            if (startDate.HasValue && endDate.HasValue)
            {
                DateTime adjustedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.StartDate >= startDate.Value && p.EndDate <= adjustedEndDate);
            }
            else if (startDate.HasValue)
            {
                query = query.Where(p => p.StartDate >= startDate.Value);
            }
            else if (endDate.HasValue)
            {
                DateTime adjustedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.EndDate <= adjustedEndDate);
                isDescending = true; // Force descending order if only endDate is provided
            }

            // **Sorting**
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = isDescending
                    ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                    : query.OrderBy(e => EF.Property<object>(e, sortBy));
            }
            else
            {
                query = query.OrderByDescending(p => p.StartDate); // Default sorting by StartDate descending
            }

            // **Pagination**
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }


        public async Task<Promotion?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.Promotions.Include(o => o.PromotionDetail)
                                            .FirstOrDefaultAsync(o => o.Id == id);
            }
            catch (Exception ex)
            {
                // Log the exception (if using logging)
                Console.WriteLine($"loi o GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<Promotion?> UpdateAsync(Guid id, Promotion promotion)
        {
            try
            {
                var existingPromotion = await _context.Promotions.FindAsync(id);
                if (existingPromotion == null)
                {
                    return null;
                }

                if (promotion.StartDate > promotion.EndDate)
                {
                    throw new Exception("StartDate không thể lớn hơn EndDate.");
                }
                existingPromotion.PromotionType = promotion.PromotionType;
                existingPromotion.IsActive = promotion.IsActive;
                existingPromotion.StartDate = promotion.StartDate;
                existingPromotion.EndDate = promotion.EndDate;
                existingPromotion.UpdateAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return existingPromotion;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật promotion.", ex);
            }
        }


        public async Task<IngredientPromotion> CreateProductPromotion(IngredientPromotion ingredientPromotion)
        {
            try
            {
                _context.IngredientPromotions.Add(ingredientPromotion);
                await _context.SaveChangesAsync();
                return ingredientPromotion;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo Ingredient Promotion.", ex);
            }
        }

        public async Task<OrderPromotion> CreateOrderPromotion (OrderPromotion orderPromotion)
        {
            try
            {
                _context.OrderPromotions.Add(orderPromotion);
                await _context.SaveChangesAsync();
                return orderPromotion;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo Order Promotion.", ex);
            }
        }

        public async Task<(List<Promotion>, int)> GetAllPromotions(
        bool isActive, string? search, string? sortBy, bool isDescending,
        PromotionType? promotionType, string? promotionCode, string? promotionName,
        DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            var query = _context.Promotions.Include(p => p.PromotionDetail).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.PromotionDetail.PromotionName.Contains(search));
            }

            if (!string.IsNullOrEmpty(promotionCode))
            {
                query = query.Where(p => p.PromotionCode == promotionCode);
            }

            if (!string.IsNullOrEmpty(promotionName))
            {
                query = query.Where(p => p.PromotionDetail.PromotionName.Contains(promotionName));
            }

            if (promotionType.HasValue)
            {
                query = query.Where(p => p.PromotionType == promotionType.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(p => p.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.EndDate <= endDate.Value);
            }

            // Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                if (isDescending)
                {
                    query = query.OrderByDescending(p => EF.Property<object>(p, sortBy));
                }
                else
                {
                    query = query.OrderBy(p => EF.Property<object>(p, sortBy));
                }
            }

            // Total count before pagination
            var total = await query.CountAsync();

            // Apply pagination
            var promotions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (promotions, total);
        }

        public async Task<List<Promotion>> GetFilteredPromotionsAsync(
    string? search, string? sortBy, bool isDescending,
    PromotionType? promotionType, string? promotionName,
    DateOnly? startDate, DateOnly? endDate, bool? isActive)
        {
            var query = _context.Promotions.Include(p => p.PromotionDetail).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.PromotionDetail.PromotionName.Contains(search));
            }

            if (promotionType.HasValue)
            {
                query = query.Where(p => p.PromotionType == promotionType.Value);
            }

            if (!string.IsNullOrEmpty(promotionName))
            {
                query = query.Where(p => p.PromotionDetail.PromotionName.Contains(promotionName));
            }

            if (startDate.HasValue || endDate.HasValue)
            {
                DateTime adjustedStart = startDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue;
                DateTime adjustedEnd = endDate?.ToDateTime(TimeOnly.MaxValue) ?? DateTime.MaxValue;
                query = query.Where(p => p.StartDate >= adjustedStart && p.EndDate <= adjustedEnd);
            }

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = isDescending
                    ? query.OrderByDescending(p => EF.Property<object>(p, sortBy))
                    : query.OrderBy(p => EF.Property<object>(p, sortBy));
            }
            else
            {
                query = query.OrderByDescending(p => p.StartDate); // Default sort by latest start date
            }

            return await query.ToListAsync();
        }


        public async Task<Promotion?> GetByCodeAsync(string code)
        {
            try
            {
                return await _context.Promotions.Include(o => o.PromotionDetail)
                                            .FirstOrDefaultAsync(o => o.PromotionCode == code);
            }
            catch (Exception ex)
            {
                // Log the exception (if using logging)
                Console.WriteLine($"loi o GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task CreateProductPromotionsBulkAsync(List<IngredientPromotion> ingredientPromotions)
        {
            if (ingredientPromotions == null || !ingredientPromotions.Any())
            {
                throw new ArgumentException("Danh sách IngredientPromotion không được rỗng.");
            }

            await _context.IngredientPromotions.AddRangeAsync(ingredientPromotions);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveProductPromotionsByPromotionIdAsync(Guid promotionId)
        {
            try
            {
                var ingredientPromotions = await _context.IngredientPromotions
                    .Where(ip => ip.PromotionId == promotionId)
                    .ToListAsync();

                if (ingredientPromotions.Any())
                {
                    _context.IngredientPromotions.RemoveRange(ingredientPromotions);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa danh sách IngredientPromotions của PromotionId: {promotionId}", ex);
            }
        }

        public async Task<Promotion> DeleteAsync(Guid id)
        {
            try
            {
                var promotion = await _context.Promotions.FindAsync(id);
                if (promotion == null)
                {
                    throw new Exception("Không tìm thấy Promotion");
                }
                promotion.IsActive = false;
                await _context.SaveChangesAsync();
                return promotion;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi ở DeleteAsync: " + ex.Message);
            }
        }

        public async Task<List<Promotion>> GetActivePromotions(PromotionType? promotionType, double? orderTotalPrice, DateOnly? expiredDate, bool? isActive)
        {
            var query = _context.Promotions
                .Include(p => p.PromotionDetail)
                .AsQueryable();

            if (promotionType.HasValue)
            {
                query = query.Where(p => p.PromotionType == promotionType.Value);
            }

            if (expiredDate.HasValue)
            {
                query = query.Where(p => p.EndDate.Date >= expiredDate.Value.ToDateTime(TimeOnly.MinValue));
            }

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            if (orderTotalPrice.HasValue)
            {
                query = query.Where(p => p.PromotionDetail != null
                                      && orderTotalPrice.Value >= p.PromotionDetail.MiniValue);
            }

            return await query.ToListAsync();
        }

        public async Task<Promotion?> GetByIdAndCode (Guid? id, string? code)
        {
            return await _context.Promotions
                .Include(p => p.PromotionDetail)
                .FirstOrDefaultAsync(p => p.Id == id && p.PromotionCode == code);
        }

    }
}
