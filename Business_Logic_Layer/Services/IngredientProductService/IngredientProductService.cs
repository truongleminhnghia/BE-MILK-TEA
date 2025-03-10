using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;


namespace Business_Logic_Layer.Services.IngredientProductService
{
    public class IngredientProductService : IIngredientProductService
    {
        private readonly IIngredientProductRepository _ingredientProductRepository;

        public IngredientProductService(IIngredientProductRepository ingredientProductRepository)
        {
            _ingredientProductRepository = ingredientProductRepository;
        }

        public async Task<IngredientProduct> CreateAsync(IngredientProduct ingredientProduct)
        {
            return await _ingredientProductRepository.CreateAsync(ingredientProduct);
        }

        public async Task<IngredientProduct> GetIngredientProductbyId(Guid ingredientId)
        {
            return await _ingredientProductRepository.GetIngredientProductbyId(ingredientId);
        }

        public async Task<bool> IngredientExistsAsync(Guid ingredientId)
        {
            return await _ingredientProductRepository.IngredientExistsAsync(ingredientId);
        }

    }
}
