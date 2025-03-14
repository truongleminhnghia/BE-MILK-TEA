using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public class IngredientQuantityRepository : IIngredientQuantityRepository
    {
        private readonly ApplicationDbContext _context;

        public IngredientQuantityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<IngredientQuantity> GetAll(
    string? search,
    Guid? ingredientId,
    ProductType? productType,
    int? minQuantity,
    int? maxQuantity,
    DateTime? startDate,
    DateTime? endDate)
        {
            var query = _context.IngredientQuantities
                .Include(iq => iq.Ingredients)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(iq => iq.Ingredients.IngredientName.Contains(search));
            }

            if (ingredientId.HasValue)
            {
                query = query.Where(iq => iq.IngredientId == ingredientId.Value);
            }

            if (productType.HasValue)
            {
                query = query.Where(iq => iq.ProductType == productType.Value);
            }

            if (minQuantity.HasValue)
            {
                query = query.Where(iq => iq.Quantity >= minQuantity.Value);
            }

            if (maxQuantity.HasValue)
            {
                query = query.Where(iq => iq.Quantity <= maxQuantity.Value);
            }

            if (startDate.HasValue || endDate.HasValue)
            {
                DateTime adjustedStart = startDate?.Date ?? DateTime.MinValue;
                DateTime adjustedEnd = endDate?.Date.AddDays(1).AddTicks(-1) ?? DateTime.MaxValue;
                query = query.Where(iq => iq.CreateAt >= adjustedStart && iq.CreateAt <= adjustedEnd);
            }

            return query;
        }

        public async Task<List<IngredientQuantity>> GetByIngredientId(Guid ingredientId)
        {
            return await _context.IngredientQuantities
                .Where(iq => iq.IngredientId == ingredientId)
                .Include(iq => iq.Ingredients)
                .ToListAsync();
        }


        public async Task AddAsync(IngredientQuantity entity)
        {
            await _context.IngredientQuantities.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(IngredientQuantity entity)
        {
            _context.IngredientQuantities.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<IngredientQuantity> GetById(Guid id)
        {
            return await _context.IngredientQuantities
            .Include(iq => iq.Ingredients)
            .FirstOrDefaultAsync(iq => iq.Id == id);
        }

        public async Task<IngredientQuantity> GetByIdAndProductType(Guid ingredientId, ProductType ProductType)
        {
            return await _context.IngredientQuantities
         .Include(iq => iq.Ingredients) // Nếu cần lấy thông tin nguyên liệu
         .FirstOrDefaultAsync(iq => iq.IngredientId == ingredientId && iq.ProductType == ProductType);
        }
    }

}
