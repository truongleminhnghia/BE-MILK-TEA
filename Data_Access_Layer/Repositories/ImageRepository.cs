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

        public async Task<IEnumerable<Image>> GetAllImagesAsync()
        {
            try
            {
                return await _context.Set<Image>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi truy xuất tất cả hình ảnh");
                throw new Exception("Không thể truy xuất hình ảnh từ cơ sở dữ liệu", ex);
            }
        }

        public async Task<Image> GetImageByIdAsync(Guid id)
        {
            try
            {
                return await _context.Set<Image>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi truy xuất hình ảnh với ID {ImageId}", id);
                throw new Exception($"Không thể truy xuất hình ảnh bằng ID {id}", ex);
            }
        }

        public async Task AddImageAsync(Image image)
        {
            try
            {
                if (image == null)
                {
                    throw new ArgumentNullException(nameof(image), "Hình ảnh không được rỗng");
                }

                await _context.Set<Image>().AddAsync(image);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogError(ex, "Đã xảy ra lỗi cơ sở dữ liệu khi thêm hình ảnh");
                throw new Exception(
                    "Không thể thêm hình ảnh vào cơ sở dữ liệu. Đã xảy ra lỗi cơ sở dữ liệu",
                    ex
                );
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi thêm hình ảnh");
                throw new Exception("Không thể thêm hình ảnh", ex);
            }
        }

        public async Task UpdateImageAsync(Image image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image), "Đối tượng hình ảnh không thể rỗng");
            }

            try
            {
                var existingImage = await _context.Images.FindAsync(image.Id);

                if (existingImage == null)
                {
                    _logger?.LogWarning(
                        "Không tìm thấy hình ảnh có ID {ImageId} để cập nhật",
                        image.Id
                    );
                    throw new KeyNotFoundException($"Hình ảnh có ID {image.Id} không tìm thấy");
                }

                existingImage.ImageUrl = image.ImageUrl;
                if (image.IngredientId != Guid.Empty)
                {
                    existingImage.IngredientId = image.IngredientId;
                }
                _context.Entry(existingImage).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger?.LogError(
                    ex,
                    "Xung đột đồng thời khi cập nhật hình ảnh với ID {ImageId}",
                    image.Id
                );
                throw new Exception(
                    $"Đã xảy ra xung đột đồng thời khi cập nhật hình ảnh có ID {image.Id}",
                    ex
                );
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogError(ex, "Database error updating image with ID {ImageId}", image.Id);
                throw new Exception(
                    $"Đã xảy ra lỗi cơ sở dữ liệu khi cập nhật hình ảnh có ID {image.Id}",
                    ex
                );
            }
            catch (KeyNotFoundException)
            {
                // Re-throw KeyNotFoundException as is
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi cập nhật hình ảnh với ID {ImageId}", image.Id);
                throw new Exception($"Lỗi cập nhật hình ảnh với ID {image.Id}", ex);
            }
        }

        public async Task DeleteImageAsync(Guid id)
        {
            try
            {
                var image = await _context.Set<Image>().FindAsync(id);
                if (image != null)
                {
                    _context.Set<Image>().Remove(image);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _logger?.LogWarning("Đã cố xóa hình ảnh không tồn tại bằng ID {ImageId}", id);
                }
            }
            catch (DbUpdateException ex)
            {
                _logger?.LogError(
                    ex,
                    "Đã xảy ra lỗi cơ sở dữ liệu khi xóa hình ảnh có ID {ImageId}",
                    id
                );
                throw new Exception(
                    $"Không xóa được hình ảnh có ID {id} Đã xảy ra lỗi cơ sở dữ liệu",
                    ex
                );
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi xóa ảnh có ID {ImageId}", id);
                throw new Exception($"Không thể xóa hình ảnh với ID {id}", ex);
            }
        }
    }
}
