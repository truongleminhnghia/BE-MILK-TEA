using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data_Access_Layer.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ImageRepository> _logger;

        public ImageRepository(ApplicationDbContext context, ILogger<ImageRepository> logger = null)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<Image> AddImageAsync(Image image)
        {
            _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<bool> CheckImageUrl(string url)
        {
            return !await _context.Images.AnyAsync(img => img.ImageUrl == url);
        }

        public async Task<bool> Delete(Guid id, Guid ingredientId)
        {
            var imageExisting = await GetIdAndIngredient(id, ingredientId);
            if (imageExisting != null)
            {
                _context.Images.Remove(imageExisting);
                return true;
            }
            return false;
        }

        public async Task<List<Image>> GetByIdAndIngredient(Guid id, Guid ingredientId)
        {
            return await _context.Images.Where(img => img.Id == id && img.IngredientId == ingredientId).ToListAsync();
        }

        public async Task<List<Image>> GetByIngredient(Guid ingredientId)
        {
            return await _context.Images.Where(img => img.IngredientId == ingredientId).ToListAsync();
        }

        public async Task<Image> GetIdAndIngredient(Guid id, Guid ingredientId)
        {
            return await _context.Images.FirstOrDefaultAsync(img => img.Id.Equals(id) && img.IngredientId.Equals(ingredientId));
        }

        public async Task<bool> Update(Guid id, Guid ingredientId, Image image)
        {
            var imageExisting = await GetIdAndIngredient(id, ingredientId);
            if (imageExisting != null)
            {
                _context.Entry(imageExisting).State = EntityState.Detached;
            }
            _context.Entry(image).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
