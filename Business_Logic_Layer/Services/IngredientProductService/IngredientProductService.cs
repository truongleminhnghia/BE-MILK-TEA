using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Business_Logic_Layer.Services.IngredientProductService
{
    public class IngredientProductService : IIngredientProductService
    {
        private readonly IIngredientProductRepository _ingredientProductRepository;
        private readonly IMapper _mapper;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IIngredientQuantityRepository _ingredientQuantityRepository;

        public IngredientProductService(IIngredientProductRepository ingredientProductRepository, IMapper mapper, IIngredientRepository ingredientRepository, IIngredientQuantityRepository ingredientQuantityRepository)
        {
            _ingredientProductRepository = ingredientProductRepository;
            _mapper = mapper;
            _ingredientRepository = ingredientRepository;
            _ingredientQuantityRepository = ingredientQuantityRepository;
        }

        private async Task<bool> CheckQuantity(int Quantity, ProductType productType, Guid id)
        {
            try
            {
                if (id == null || Quantity == 0 || productType == null)
                {
                    throw new Exception("số lượng không được nhỏ hơn 0 hoặc loại sản phẩm ko được trống");
                }
                var result = await _ingredientQuantityRepository.GetByIdAndProductType(id, productType);
                if (result == null)
                {
                    throw new Exception("Không tồn tại số lượng dựa trên loại yêu cầu");
                }
                else if (result != null && Quantity > result.Quantity)
                {
                    throw new Exception($"Số lượng của loại {productType.ToString()} không còn đủ");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: ", ex.Message);
                return false;
            }
        }

        public async Task<IngredientProductResponse> CreateAsync(IngredientProductRequest request, bool isCart)
        {
            try
            {
                if (request == null) throw new ArgumentNullException("Dữ liệu sản phẩm không hợp lệ");
                // if (request.Id != null) request.Id = null;
                if (request.IngredientId == null) throw new ArgumentNullException("Nguyên liệu không hợp lệ");
                var ingredient = await _ingredientRepository.GetById(request.IngredientId);
                if (ingredient == null)
                {
                    throw new Exception("Ingredient no existing");
                }
                var newProduct = _mapper.Map<IngredientProduct>(request);
                if (isCart)
                {
                    newProduct.TotalPrice = 0;
                }
                bool result = await CheckQuantity(request.Quantity, request.ProductType, ingredient.Id);
                if (!result)
                {
                    throw new Exception("Số lượng không phù hợp");
                }
                // newProduct.TotalPrice = request.Quantity * ingredientExists.PriceOrigin;
                var created = await _ingredientProductRepository.CreateAsync(newProduct);
                var response = _mapper.Map<IngredientProductResponse>(created);
                if (isCart)
                {
                    double price = 0.0;
                    if (ingredient.PricePromotion > 0)
                    {
                        price = ingredient.PricePromotion * request.Quantity;
                    }
                    else
                    {
                        price = ingredient.PriceOrigin * request.Quantity;
                    }
                    response.TotalPrice = price;
                }
                response.Id = created.Id;
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo IngredientProduct: " + ex.Message);
            }
        }

        public async Task<IngredientProductResponse> GetIngredientProductbyId(Guid ingredientProductId)
        {
            try
            {
                IngredientProductResponse ingredientProductResponse = new IngredientProductResponse();
                var ingredientProduct = await _ingredientProductRepository.GetIngredientProductbyId(ingredientProductId);
                if(ingredientProduct != null) {
                    var ingredient = await _ingredientRepository.GetById(ingredientProduct.IngredientId);
                    if(ingredient != null) {
                        ingredientProductResponse.Ingredient = ingredient;
                    }
                }
                ingredientProductResponse = _mapper.Map<IngredientProductResponse>(ingredientProduct);
                return ingredientProductResponse;
            }
            catch (Exception ex)
            {
                throw new Exception("Không thể lấy thông tin Ingredient Product: " + ex.Message);
            }
        }

        public async Task<IngredientProductResponse> UpdateAsync(Guid id, IngredientProductRequest request)
        {
            try
            {
                var ingredientProduct = await _ingredientProductRepository.GetIngredientProductbyId(id);
                if (ingredientProduct == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy sản phẩm");
                }
                var ingredientExists = await _ingredientRepository.GetById(request.IngredientId);
                if (ingredientExists == null) throw new KeyNotFoundException("Nguyên liệu không tồn tại");
                // request.Id = id;
                _mapper.Map(request, ingredientProduct);
                ingredientProduct.TotalPrice = request.Quantity * ingredientExists.PriceOrigin;
                await _ingredientProductRepository.UpdateAsync(ingredientProduct);
                return _mapper.Map<IngredientProductResponse>(ingredientProduct);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật IngredientProduct: " + ex.Message);
            }
        }

        public async Task<bool> IngredientExistsAsync(Guid ingredientId)
        {
            return await _ingredientProductRepository.IngredientExistsAsync(ingredientId);
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
