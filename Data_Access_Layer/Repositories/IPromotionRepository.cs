using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Repositories
{
    public interface IPromotionRepository
    {
        Task<Promotion> CreatePromotionAsync(Promotion promotion);
    }
}