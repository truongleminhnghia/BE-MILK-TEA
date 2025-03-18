using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public class IngredientProductRepository : IIngredientProductRepository
    {
        private readonly ApplicationDbContext _context;

        public IngredientProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IngredientProduct> CreateAsync(IngredientProduct ingredientProduct)
        {
            _context.IngredientProducts.Add(ingredientProduct);
            await _context.SaveChangesAsync();
            return ingredientProduct;
        }

        public async Task<IngredientProduct> UpdateAsync(IngredientProduct ingredientProduct)
        {
            _context.IngredientProducts.Update(ingredientProduct);
            await _context.SaveChangesAsync();
            return ingredientProduct;
        }

        public async Task<bool> IngredientExistsAsync(Guid ingredientId)
        {
            return await _context.Ingredients.AnyAsync(i => i.Id == ingredientId);
        }

        public async Task<IngredientProduct> GetIngredientProductbyId(Guid ingredientProductId)
        {
            return await _context.IngredientProducts.FirstOrDefaultAsync(n => n.Id.Equals(ingredientProductId));
        }

        public async Task<IngredientProduct> UpdateAsync(Guid ingredientProductId, IngredientProduct ingredientProduct)
        {
            var existingProduct = await _context.IngredientProducts.FirstOrDefaultAsync(p => p.Id == ingredientProductId);

            if (existingProduct == null)
            {
                throw new KeyNotFoundException("không tìm thấy Ingredient Product.");
            }

            // Cập nhật giá trị của existingProduct từ dữ liệu đầu vào
            existingProduct.TotalPrice = ingredientProduct.TotalPrice;
            existingProduct.Quantity = ingredientProduct.Quantity;
            existingProduct.ProductType = ingredientProduct.ProductType;

            _context.IngredientProducts.Update(existingProduct);
            // Lưu thay đổi vào database
            await _context.SaveChangesAsync();

            return existingProduct;
        }
        public async Task<IEnumerable<IngredientProduct>> GetAllAsync(Guid? ingredientId, int page, int pageSize)
        {
            var query = _context.IngredientProducts.AsQueryable();

            if (ingredientId.HasValue)
            {
                query = query.Where(ip => ip.IngredientId == ingredientId.Value);
            }

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public IQueryable<IngredientProduct> Query()
        {
            return _context.IngredientProducts.AsQueryable();
        }

        public async Task<bool> DeleteAsync(IngredientProduct ingredientProduct)
        {
            _context.IngredientProducts.Remove(ingredientProduct);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<IngredientProduct>> GetByIngredientIdAsync(Guid ingredientId)
        {
            return await _context.IngredientProducts
                .Where(ip => ip.IngredientId == ingredientId)
                .Include(ip => ip.Ingredient)
                .ToListAsync();
        }
    }
}
