using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public class PromotionDetailRepository : IPromotionDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public PromotionDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PromotionDetail>> GetAll()
        {
            return await _context.PromotionDetails.ToListAsync();
        }

        public async Task<PromotionDetail?> GetById(Guid id)
        {
            return await _context.PromotionDetails.FindAsync(id);
        }

        public async Task Create(PromotionDetail promotionDetail)
        {
            _context.PromotionDetails.Add(promotionDetail);
            await _context.SaveChangesAsync();
        }

        public async Task Update(PromotionDetail promotionDetail)
        {
            _context.PromotionDetails.Update(promotionDetail);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var entity = await _context.PromotionDetails.FindAsync(id);
            if (entity != null)
            {
                _context.PromotionDetails.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
