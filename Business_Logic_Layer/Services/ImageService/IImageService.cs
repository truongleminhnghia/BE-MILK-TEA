using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business_Logic_Layer.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Business_Logic_Layer.Services
{
    public interface IImageService
    {
        Task<IEnumerable<Image>> GetAllImagesAsync();
        Task<Image> GetImageByIdAsync(Guid id);
        Task AddImageAsync(Image image);
        Task UpdateImageAsync(Guid id, Image image);
        Task DeleteImageAsync(Guid id);
    }
}
