using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer.Repositories
{
    public class PromotionDetailRepository : IPromotionDetailRepository
    {
        private readonly ApplicationDbContext _context;
        public PromotionDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PromotionDetail> CreateAsync(PromotionDetail promotionDetail)
        {
            promotionDetail.Id = Guid.NewGuid();
            _context.PromotionDetails.Add(promotionDetail);
            await _context.SaveChangesAsync();
            return promotionDetail;
        }

        public async Task<bool> DeleteByIdAsync(Guid id)
        {
            var promotionDetail = await _context.PromotionDetails.FindAsync(id);
            if (promotionDetail == null)
            {
                return false;
            }
            _context.PromotionDetails.Remove(promotionDetail);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PromotionDetail>> GetAllPromotionDetailAsync(Guid id)
        {
            try
            {
                return await _context.PromotionDetails
            .Where(pd => pd.PromotionId == id)
            .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể lọc được promotion detail: {ex.Message}", ex);
            }
        }

        public async Task<PromotionDetail> GetbyId(Guid id)
        {
            return await _context.PromotionDetails.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<PromotionDetail> GetbyPromotionId(Guid promotionId)
        {
            return await _context.PromotionDetails.FirstOrDefaultAsync(c => c.PromotionId == promotionId);
        }
        public async Task<PromotionDetail> UpdateAsync(Guid id, PromotionDetail promotionDetail)
        {
            var existingPromotionDetail = await _context.PromotionDetails.FindAsync(id);
            if (existingPromotionDetail == null)
            {
                return null;
            }
            existingPromotionDetail.PromotionName = promotionDetail.PromotionName;
            existingPromotionDetail.Description = promotionDetail.Description;
            existingPromotionDetail.DiscountValue = promotionDetail.DiscountValue;
            existingPromotionDetail.MiniValue = promotionDetail.MiniValue;
            existingPromotionDetail.MaxValue = promotionDetail.MaxValue;
            await _context.SaveChangesAsync();
            return existingPromotionDetail;
        }
    }
}
