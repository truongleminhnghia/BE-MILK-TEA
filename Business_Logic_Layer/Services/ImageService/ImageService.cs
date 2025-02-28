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

        public async Task<List<ImageRespone>> AddImages(Guid ingredientId, List<ImageRequest> request)
        {
            try
            {
                List<Image> images = new List<Image>();
                Ingredient ingredientExisting = await _ingredientRepository.GetIngredientByIdAsync(ingredientId);
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
                return _mapper.Map<List<ImageRespone>>(images);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return null;
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
                throw new Exception($"Không thể truy xuất hình ảnh có ID {id}", ex);
            }
        }

        // public async Task UpdateImageAsync(Guid id, ImageRespone imageResponse)
        // {
        //     try
        //     {
        //         if (imageResponse == null)
        //         {
        //             throw new ArgumentNullException(
        //                 nameof(imageResponse),
        //                 "Phản hồi hình ảnh không thể rỗng"
        //             );
        //         }

        //         var existingImage = await _imageRepository.GetImageByIdAsync(id);
        //         if (existingImage == null)
        //         {
        //             throw new Exception($"Không tìm thấy hình ảnh với ID {id}");
        //         }

        //         if (
        //             imageResponse.IngredientId != existingImage.IngredientId
        //             && imageResponse.IngredientId != Guid.Empty
        //         )
        //         {
        //             var ingredient = await _ingredientRepository.GetByIdAsync(
        //                 imageResponse.IngredientId
        //             );
        //             if (ingredient == null)
        //             {
        //                 throw new Exception(
        //                     $"IngredientId không hợp lệ {imageResponse.IngredientId}. Thành phần được chỉ định không tồn tại."
        //                 );
        //             }
        //         }
        //         var updatedImage = _mapper.Map<Image>(imageResponse);
        //         updatedImage.Id = id;
        //         await _imageRepository.UpdateImageAsync(updatedImage);
        //     }
        //     catch (ArgumentNullException)
        //     {
        //         throw;
        //     }
        //     catch (KeyNotFoundException ex)
        //     {
        //         throw new Exception($"Không tìm thấy hình ảnh với ID {id}", ex);
        //     }
        //     catch (Exception ex)
        //     {
        //         throw new Exception($"Không cập nhật được hình ảnh có ID {id}", ex);
        //     }
        // }
    }
}
