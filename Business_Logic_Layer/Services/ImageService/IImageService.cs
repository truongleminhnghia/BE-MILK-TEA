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
        Task<List<ImageRespone>> GetByIngredient(Guid ingredientId);
        Task<ImageRespone> GetById(Guid id);
        Task<Image> AddImageAsync(Image image);
        Task<List<ImageRespone>> AddImages(Guid ingredientId, List<ImageRequest> request);
        Task<ImageRespone> UpdateImageAsync(Guid id, Guid ingredientId, ImageRequest request);
        Task<List<ImageRespone>> UpdateImages(List<ImageRequest> request, Guid ingredientId);
        Task<bool> DeleteImageAsync(Guid id, Guid ingredientId);
    }

}
