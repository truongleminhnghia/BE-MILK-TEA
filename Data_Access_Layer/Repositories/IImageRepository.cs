using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Repositories
{
    public interface IImageRepository
    {
        Task<IEnumerable<Image>> GetAllImagesAsync();
        Task<Image> GetImageByIdAsync(Guid id);
        Task<Image> AddImageAsync(Image image);
        Task UpdateImageAsync(Image image); // Changed to accept only one parameter
        Task DeleteImageAsync(Guid id);
        Task<bool> CheckImageUrl(string url);
    }
}
