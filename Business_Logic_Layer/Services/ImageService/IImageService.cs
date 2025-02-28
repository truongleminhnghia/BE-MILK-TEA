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
        Task<IEnumerable<ImageRespone>> GetAllImagesAsync();
        Task<ImageRespone> GetImageByIdAsync(Guid id);
        Task<Image> AddImageAsync(Image image);
        Task<List<ImageRespone>> AddImages(Guid ingredientId, List<ImageRequest> request);
        // Task UpdateImageAsync(Guid id, ImageRespone imageResponse);
        Task DeleteImageAsync(Guid id);
    }

}
