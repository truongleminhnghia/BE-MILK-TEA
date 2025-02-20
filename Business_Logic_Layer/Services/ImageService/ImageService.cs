using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public async Task AddImageAsync(Models.Image image)
        {
            // Check if the IngredientId exists
            var ingredient = await _ingredientRepository.GetByIdAsync(image.IngredientId);
            if (ingredient == null)
            {
                throw new Exception(
                    "Invalid IngredientId. The specified Ingredient does not exist."
                );
            }

            var mappedImage = _mapper.Map<Data_Access_Layer.Entities.Image>(image);
            await _imageRepository.AddImageAsync(mappedImage);
        }

        public async Task DeleteImageAsync(Guid id)
        {
            await _imageRepository.DeleteImageAsync(id);
        }

        public async Task<IEnumerable<Models.Image>> GetAllImagesAsync()
        {
            var images = await _imageRepository.GetAllImagesAsync();
            return _mapper.Map<IEnumerable<Models.Image>>(images);
        }

        public async Task<Models.Image> GetImageByIdAsync(Guid id)
        {
            var image = await _imageRepository.GetImageByIdAsync(id);
            return _mapper.Map<Models.Image>(image);
        }

        public async Task UpdateImageAsync(Models.Image image)
        {
            var mappedImage = _mapper.Map<Data_Access_Layer.Entities.Image>(image);
            await _imageRepository.UpdateImageAsync(mappedImage);
        }
    }
}
