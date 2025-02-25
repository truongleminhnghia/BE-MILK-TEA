using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Image>> GetAllImagesAsync()
        {
            return await _context.Set<Image>().ToListAsync();
        }

        public async Task<Image> GetImageByIdAsync(Guid id)
        {
            return await _context.Set<Image>().FindAsync(id);
        }

        public async Task AddImageAsync(Image image)
        {
            await _context.Set<Image>().AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateImageAsync(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image), "Image object cannot be null.");
            }

            var existingImage = await _context.Images.FindAsync(image.Id);

            if (existingImage == null)
            {
                throw new KeyNotFoundException($"Image with ID {image.Id} not found.");
            }

            // Update properties from the incoming image object
            existingImage.ImageUrl = image.ImageUrl;
            if (image.IngredientId != Guid.Empty)
            {
                existingImage.IngredientId = image.IngredientId;
            }
            // Add other properties as needed

            // Mark as modified
            _context.Entry(existingImage).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteImageAsync(Guid id)
        {
            var image = await _context.Set<Image>().FindAsync(id);
            if (image != null)
            {
                _context.Set<Image>().Remove(image);
                await _context.SaveChangesAsync();
            }
        }
    }
}
