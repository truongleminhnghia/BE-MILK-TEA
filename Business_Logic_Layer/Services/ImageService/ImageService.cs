using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services.IngredientService
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;
        private readonly IIngredientRepository _ingredientRepository;

        public ImageService(
            IImageRepository imageRepository,
            IIngredientRepository ingredientRepository,
            IMapper mapper
        )
        {
            _imageRepository = imageRepository;
            _mapper = mapper;
            _ingredientRepository = ingredientRepository;
        }

        public async Task AddImageAsync(ImageRespone image)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(image.IngredientId);
            if (ingredient == null)
            {
                throw new Exception(
                    "Invalid IngredientId. The specified Ingredient does not exist."
                );
            }

            var mappedImage = _mapper.Map<Image>(image);
            await _imageRepository.AddImageAsync(mappedImage);
        }

        public async Task DeleteImageAsync(Guid id)
        {
            await _imageRepository.DeleteImageAsync(id);
        }

        public async Task<IEnumerable<ImageRespone>> GetAllImagesAsync()
        {
            var images = await _imageRepository.GetAllImagesAsync();
            return _mapper.Map<IEnumerable<ImageRespone>>(images);
        }

        public async Task<ImageRespone> GetImageByIdAsync(Guid id)
        {
            var image = await _imageRepository.GetImageByIdAsync(id);
            if (image == null)
            {
                return null;
            }
            return _mapper.Map<ImageRespone>(image);
        }

        public async Task UpdateImageAsync(Guid id, ImageRespone imageResponse)
        {
            // First check if image exists
            var existingImage = await _imageRepository.GetImageByIdAsync(id);
            if (existingImage == null)
            {
                throw new Exception("Không tìm thấy hình ảnh với ID này.");
            }

            // Create a new image entity with updated values
            var updatedImage = _mapper.Map<Image>(imageResponse);

            // Ensure the ID is preserved
            updatedImage.Id = id;

            // Call repository with the updated image only
            await _imageRepository.UpdateImageAsync(updatedImage);
        }
    }
}
