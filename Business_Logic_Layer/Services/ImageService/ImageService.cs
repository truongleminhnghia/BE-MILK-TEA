using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.Extensions.Logging;

namespace Business_Logic_Layer.Services.IngredientService
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly ILogger<ImageService> _logger;

        public ImageService(
            IImageRepository imageRepository,
            IIngredientRepository ingredientRepository,
            IMapper mapper,
            ILogger<ImageService> logger = null
        )
        {
            _imageRepository = imageRepository;
            _mapper = mapper;
            _ingredientRepository = ingredientRepository;
            _logger = logger;
        }

        public async Task AddImageAsync(ImageRespone image)
        {
            try
            {
                if (image == null)
                {
                    throw new ArgumentNullException(nameof(image), "Hình ảnh không được rỗng");
                }

                var ingredient = await _ingredientRepository.GetByIdAsync(image.IngredientId);
                if (ingredient == null)
                {
                    _logger?.LogWarning(
                        "IngredientId {IngredientId} được cung cấp không hợp lệ",
                        image.IngredientId
                    );
                    throw new Exception(
                        $"Id thành phần không hợp lệ {image.IngredientId}. Thành phần được chỉ định không tồn tại."
                    );
                }

                var mappedImage = _mapper.Map<Image>(image);
                await _imageRepository.AddImageAsync(mappedImage);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(
                    ex,
                    "Lỗi thêm hình ảnh với IngredientId {IngredientId}",
                    image?.IngredientId.ToString() ?? "null"
                );
                throw new Exception("Không thể thêm hình ảnh", ex);
            }
        }

        public async Task DeleteImageAsync(Guid id)
        {
            try
            {
                await _imageRepository.DeleteImageAsync(id);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi xóa ảnh có ID {ImageId}", id);
                throw new Exception($"Không thể xóa hình ảnh có ID {id}", ex);
            }
        }

        public async Task<IEnumerable<ImageRespone>> GetAllImagesAsync()
        {
            try
            {
                var images = await _imageRepository.GetAllImagesAsync();
                return _mapper.Map<IEnumerable<ImageRespone>>(images);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi truy xuất tất cả hình ảnh");
                throw new Exception("Không thể truy xuất hình ảnh", ex);
            }
        }

        public async Task<ImageRespone> GetImageByIdAsync(Guid id)
        {
            try
            {
                var image = await _imageRepository.GetImageByIdAsync(id);
                if (image == null)
                {
                    return null;
                }
                return _mapper.Map<ImageRespone>(image);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi truy xuất hình ảnh có ID {ImageId}", id);
                throw new Exception($"Không thể truy xuất hình ảnh có ID {id}", ex);
            }
        }

        public async Task UpdateImageAsync(Guid id, ImageRespone imageResponse)
        {
            try
            {
                if (imageResponse == null)
                {
                    throw new ArgumentNullException(
                        nameof(imageResponse),
                        "Phản hồi hình ảnh không thể rỗng"
                    );
                }

                var existingImage = await _imageRepository.GetImageByIdAsync(id);
                if (existingImage == null)
                {
                    _logger?.LogWarning("Không tìm thấy hình ảnh có ID {ImageId} để cập nhật", id);
                    throw new Exception($"Không tìm thấy hình ảnh với ID {id}");
                }

                if (
                    imageResponse.IngredientId != existingImage.IngredientId
                    && imageResponse.IngredientId != Guid.Empty
                )
                {
                    var ingredient = await _ingredientRepository.GetByIdAsync(
                        imageResponse.IngredientId
                    );
                    if (ingredient == null)
                    {
                        _logger?.LogWarning(
                            "IngredientId {IngredientId} được cung cấp không hợp lệ để cập nhật",
                            imageResponse.IngredientId
                        );
                        throw new Exception(
                            $"IngredientId không hợp lệ {imageResponse.IngredientId}. Thành phần được chỉ định không tồn tại."
                        );
                    }
                }
                var updatedImage = _mapper.Map<Image>(imageResponse);
                updatedImage.Id = id;
                await _imageRepository.UpdateImageAsync(updatedImage);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (KeyNotFoundException ex)
            {
                _logger?.LogWarning(
                    ex,
                    "Không tìm thấy hình ảnh có ID {ImageId} trong quá trình cập nhật ở cấp kho lưu trữ",
                    id
                );
                throw new Exception($"Không tìm thấy hình ảnh với ID {id}", ex);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Lỗi cập nhật hình ảnh với ID {ImageId}", id);
                throw new Exception($"Không cập nhật được hình ảnh có ID {id}", ex);
            }
        }
    }
}
