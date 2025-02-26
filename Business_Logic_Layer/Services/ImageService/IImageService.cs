using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business_Logic_Layer.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Business_Logic_Layer.Services
{
    public interface IImageService
    {
        Task<IEnumerable<ImageRespone>> GetAllImagesAsync();
        Task<ImageRespone> GetImageByIdAsync(Guid id);
        Task AddImageAsync(ImageRespone image);
        Task UpdateImageAsync(Guid id, ImageRespone imageResponse);
        Task DeleteImageAsync(Guid id);
    }
}
