using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Business_Logic_Layer.Services.IngredientProductService
{
    public class IngredientProductService : IIngredientProductService
    {
        private readonly IIngredientProductRepository _ingredientProductRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public IngredientProductService(IIngredientProductRepository ingredientProductRepository, IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _ingredientProductRepository = ingredientProductRepository;
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        public async Task<IngredientProduct> CreateAsync(IngredientProductRequest request)
        {
            var ingredient = await _ingredientRepository.GetById(request.IngredientId);
            if (ingredient == null)
            {
                throw new KeyNotFoundException("Không tìm thấy nguyên liệu.");
            }
            var totalPrice = request.Quantity * ingredient.PriceOrigin;

            var ingredientProduct = new IngredientProduct
            {
                IngredientId = request.IngredientId,
                Quantity = request.Quantity,
                ProductType = request.ProductType,
                TotalPrice = totalPrice
            };

            return await _ingredientProductRepository.CreateAsync(ingredientProduct);
        }

        public async Task<IngredientProduct> GetIngredientProductbyId(Guid ingredientProductId)
        {
            try
            {
                return await _ingredientProductRepository.GetIngredientProductbyId(ingredientProductId);
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy thông tin Ingredient Product bằng ID", ex);
            }
        }
        

        public async Task<bool> IngredientExistsAsync(Guid ingredientId)
        {
            return await _ingredientProductRepository.IngredientExistsAsync(ingredientId);
        }

        public async Task<IngredientProduct> UpdateAsync(Guid ingredientProductId, IngredientProductRequest request)
        {
            try
            {
                var existingIngredientProduct = await _ingredientProductRepository.GetIngredientProductbyId(ingredientProductId);
                if (existingIngredientProduct == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy IngredientProduct");
                }

                var ingredient = await _ingredientRepository.GetById(existingIngredientProduct.IngredientId);
                if (ingredient == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy Ingredient");
                }
                existingIngredientProduct.Quantity = request.Quantity;
                existingIngredientProduct.ProductType = request.ProductType;
                existingIngredientProduct.TotalPrice = request.Quantity * ingredient.PriceOrigin;

                var updatedIngredientProduct = await _ingredientProductRepository.UpdateAsync(ingredientProductId, existingIngredientProduct);
                return updatedIngredientProduct;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể update quantity, price, type", ex);
            }
        }
        public async Task<IEnumerable<IngredientProduct>> GetAllAsync(Guid? ingredientId, int page, int pageSize)
        {
            var query = _ingredientProductRepository.Query();

            if (ingredientId.HasValue)
            {
                query = query.Where(ip => ip.IngredientId == ingredientId.Value);
            }

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

    }
}
