using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer.Repositories
{
    public class IngredientReviewRepository : IIngredientReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public IngredientReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<IngredientReview>> GetAllAsync()
        {
            return await _context
                .IngredientReviews.Include(r => r.Ingredient)
                .Include(r => r.Account)
                .ToListAsync();
        }

        public async Task<IEnumerable<IngredientReview>> GetByIngredientIdAsync(Guid ingredientId)
        {
            return await _context
                .IngredientReviews.Where(r => r.IngredientId == ingredientId)
                .Include(r => r.Ingredient)
                .Include(r => r.Account)
                .ToListAsync();
        }

        public async Task<IEnumerable<IngredientReview>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context
                .IngredientReviews.Where(r => r.AccountId == accountId)
                .Include(r => r.Ingredient)
                .Include(r => r.Account)
                .ToListAsync();
        }

        public async Task<IngredientReview> GetByIdAsync(Guid id)
        {
            return await _context
                .IngredientReviews.Include(r => r.Ingredient)
                .Include(r => r.Account)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IngredientReview> CreateAsync(IngredientReview ingredientReview)
        {
            await _context.IngredientReviews.AddAsync(ingredientReview);
            await _context.SaveChangesAsync();
            return ingredientReview;
        }

        public async Task<IngredientReview> UpdateAsync(IngredientReview ingredientReview)
        {
            _context.IngredientReviews.Update(ingredientReview);
            await _context.SaveChangesAsync();
            return ingredientReview;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var review = await _context.IngredientReviews.FindAsync(id);
            if (review == null)
                return false;

            _context.IngredientReviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.IngredientReviews.AnyAsync(r => r.Id == id);
        }

        public async Task<bool> ExistsReviewByAccountAndIngredientAsync(
            Guid accountId,
            Guid ingredientId
        )
        {
            return await _context.IngredientReviews.AnyAsync(r =>
                r.AccountId == accountId && r.IngredientId == ingredientId
            );
        }
    }
}
