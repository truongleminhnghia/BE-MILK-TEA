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

        public Task<bool> DeleteImageAsync(Guid id, Guid ingredientId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ImageRespone>> GetByIdAndIngredient(Guid id, Guid ingredientId)
        {
            try
            {
                if (ingredientId == null)
                {
                    throw new Exception("idImage hoặc IdIngredient không được phép null");
                }
                else if (id.Equals("") || ingredientId != null)
                {
                    return _mapper.Map<List<ImageRespone>>(await _imageRepository.GetByIngredient(ingredientId));
                }
                return _mapper.Map<List<ImageRespone>>(await _imageRepository.GetByIdAndIngredient(id, ingredientId));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return null;
            }
        }

        public async Task<ImageRespone> UpdateImageAsync(Guid id, Guid ingredientId, ImageRequest request)
        {
            try
            {
                bool result = await _imageRepository.Update(id, ingredientId, _mapper.Map<Image>(request));
                if (!result)
                {
                    throw new Exception("Cập nhật thất bại");
                }
                return _mapper.Map<ImageRespone>(await _imageRepository.GetIdAndIngredient(id, ingredientId));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return null;
            }
        }

        public async Task<List<ImageRespone>> UpdateImages(List<ImageRequest> request, Guid ingredientId)
        {
            try
            {
                List<ImageRespone> imageRespones = new List<ImageRespone>();
                foreach (var item in request)
                {
                    var imageExisting = await _imageRepository.GetIdAndIngredient(item.Id, ingredientId);
                    if (imageExisting == null)
                    {
                        throw new Exception("Hình Ảnh không tồn tại");
                    }
                    imageExisting.ImageUrl = item.ImageUrl;
                    var imageModified = _mapper.Map<ImageRespone>(imageExisting);
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
