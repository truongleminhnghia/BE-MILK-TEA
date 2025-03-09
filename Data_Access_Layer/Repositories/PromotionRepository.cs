using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer.Repositories
{
    public interface IPromotionRepository
    {
        Task<IEnumerable<Promotion>> GetAllPromotionAsync(
            bool? IsActive, string? search, string? sortBy, 
            bool isDescending,PromotionType? promotionType,
            DateTime? startDate, DateTime? endDate,
            int page, int pageSize);
        Task<Promotion?> GetByIdAsync(Guid id);
        Task<Promotion> CreateAsync(Promotion promotion);
        Task<Promotion?> UpdateAsync(Guid id, Promotion promotion);
        //Task<bool> DeleteAsync(Guid id);
    }
    public class PromotionRepository : IPromotionRepository
    {
        private readonly ApplicationDbContext _context;

        public PromotionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Promotion> CreateAsync(Promotion promotion)
        {
            if (promotion.StartDate > promotion.EndDate)
            {
                throw new ArgumentException("StartDate không thể lớn hơn EndDate.");
            }

            try
        {
                _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();
            return promotion;
        }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo promotion.", ex);
            }
        }



        public async Task<IEnumerable<Promotion>> GetAllPromotionAsync(
            bool? IsActive,string? search, string? sortBy, bool isDescending,
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
    }
}
