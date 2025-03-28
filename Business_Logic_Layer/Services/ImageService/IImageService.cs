using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Models.Requests;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Business_Logic_Layer.Services
{
    public interface IImageService
    {
        // Task<IEnumerable<ImageRespone>> GetAllImagesAsync();
        Task<List<ImageResponse>> GetByIngredient(Guid ingredientId);
        Task<ImageResponse> GetById(Guid id);
        Task<Image> AddImageAsync(Image image);
        Task<List<ImageResponse>> AddImages(Guid ingredientId, List<ImageRequest> request);
        Task<ImageResponse> UpdateImageAsync(Guid id, Guid ingredientId, ImageRequest request);
        Task<List<ImageResponse>> UpdateImages(List<ImageRequest> request, Guid ingredientId);
        Task<bool> DeleteImageAsync(Guid id, Guid ingredientId);
    }

}
