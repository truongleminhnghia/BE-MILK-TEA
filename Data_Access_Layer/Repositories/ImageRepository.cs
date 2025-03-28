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

        public ImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<Image> AddImageAsync(Image image)
        {
            _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
            return image;
        }


        public async Task<bool> Delete(Guid id)
        {
            _context.Images.Remove(await _context.Images.FirstOrDefaultAsync(i => i.Id == id));
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Image> GetById(Guid id)
        {
            return await _context.Images.FirstAsync(i => i.Id == id);
        }

        public async Task<List<Image>> GetByIdAndIngredientByList(Guid id, Guid ingredientId)
        {
            return await _context.Images.Where(img => img.Id == id && img.IngredientId == ingredientId).ToListAsync();
        }

        public async Task<List<Image>> GetByIngredient(Guid ingredientId)
        {
            return await _context.Images.Where(img => img.IngredientId == ingredientId).ToListAsync();
        }

        public async Task<Image> GetIdAndIngredient(Guid? id, Guid ingredientId)
        {
            return await _context.Images.FirstOrDefaultAsync(img => img.Id.Equals(id) && img.IngredientId.Equals(ingredientId));
        }

        public async Task<bool> Update(Guid id, Image image)
        {
            var imageExisting = await _context.Images.FirstOrDefaultAsync(i => i.Id == id);
            if (imageExisting == null)
            {
                return false;
            }
            imageExisting.ImageUrl = image.ImageUrl;
            _context.Images.Update(imageExisting);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
