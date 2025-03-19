using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Models.Requests;
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

        public ImageService(IImageRepository imageRepository, IMapper mapper, IIngredientRepository IIngredientRepository)
        {
            _imageRepository = imageRepository;
            _mapper = mapper;
            _ingredientRepository = IIngredientRepository;
        }

        public async Task<Image> AddImageAsync(Image _image)
        {
            try
            {
                if (_image == null)
                {
                    throw new Exception("Không có hình ảnh");
                }
                Image image = await _imageRepository.AddImageAsync(_image);
                if (image == null)
                {
                    throw new Exception("Tạo ảnh không thành công");
                }
                return image;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể thêm hình ảnh", ex);
            }
        }

        public async Task<List<ImageResponse>> AddImages(Guid ingredientId, List<ImageRequest> request)
        {
            try
            {
                List<Image> images = new List<Image>();
                Ingredient ingredientExisting = await _ingredientRepository.GetById(ingredientId);
                if (ingredientExisting == null)
                {
                    throw new Exception("Nguyên liệu không tồn tại");
                }
                foreach (var item in request)
                {
                    Image image = new Image
                    {
                        ImageUrl = item.ImageUrl,
                        Ingredient = ingredientExisting
                    };

                    Image savedImage = await AddImageAsync(image);
                    images.Add(image);
                }
                return _mapper.Map<List<ImageResponse>>(images);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return null;
            }
        }

        public async Task<bool> DeleteImageAsync(Guid id, Guid ingredientId)
        {
            try
            {
                bool result;
                var image = await _imageRepository.GetById(id);
                if (image == null)
                {
                    throw new Exception("Hình ảnh không tồn tại");
                }
                if (image.IngredientId.Equals(ingredientId))
                {
                    result = await _imageRepository.Delete(image.Id);
                }
                else
                {
                    result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return false;
            }
        }

        public async Task<ImageResponse> GetById(Guid id)
        {
            try
            {
                var image = await _imageRepository.GetById(id);
                if (image == null)
                {
                    throw new Exception("Hình ảnh không tồn tại");
                }
                return _mapper.Map<ImageResponse>(image);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return null;
            }
        }

        public async Task<List<ImageResponse>> GetByIngredient(Guid ingredientId)
        {
            try
            {
                if (ingredientId == null)
                {
                    throw new Exception("IdIngredient không được phép null");
                }
                return _mapper.Map<List<ImageResponse>>(await _imageRepository.GetByIngredient(ingredientId));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return null;
            }
        }

        public async Task<ImageResponse> UpdateImageAsync(Guid id, Guid ingredientId, ImageRequest request)
        {
            try
            {
                bool result = await _imageRepository.Update(id, _mapper.Map<Image>(request));
                if (!result)
                {
                    throw new Exception("Cập nhật thất bại");
                }
                return _mapper.Map<ImageResponse>(await _imageRepository.GetIdAndIngredient(id, ingredientId));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return null;
            }
        }

        public async Task<List<ImageResponse>> UpdateImages(List<ImageRequest> request, Guid ingredientId)
        {
            try
            {
                List<ImageResponse> imageRespones = new List<ImageResponse>();
                foreach (var item in request)
                {
                    var imageExisting = await _imageRepository.GetIdAndIngredient(item.Id, ingredientId);
                    if (imageExisting == null)
                    {
                        throw new Exception("Hình Ảnh không tồn tại");
                    }
                    imageExisting.ImageUrl = item.ImageUrl;
                    var imageModified = _mapper.Map<ImageResponse>(imageExisting);
                    imageRespones.Add(imageModified);
                }
                if (imageRespones == null)
                {
                    throw new Exception("Update thất bại");
                }
                return imageRespones;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return null;
            }
        }
    }
}
