using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.Services.Interfaces
{
    public interface ICartItemService
    {
        Task<IEnumerable<CartItem>> GetAll();
        Task<CartItem> GetById(Guid id);
        Task<CartItem> Create(CartItem cartItem);
        Task<CartItem> Update(CartItem cartItem);
        Task<bool> Delete(Guid id);

    }
}
