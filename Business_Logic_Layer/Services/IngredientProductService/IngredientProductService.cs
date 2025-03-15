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

        public IngredientProductService(IIngredientProductRepository ingredientProductRepository, IMapper mapper, IIngredientRepository ingredientRepository)
        {
            _ingredientProductRepository = ingredientProductRepository;
            _mapper = mapper;
            _ingredientRepository = ingredientRepository;
        }

        public async Task<IngredientProductResponse> CreateAsync(IngredientProductRequest request)
        {
            try
            {
                if (request == null) throw new ArgumentNullException("Dữ liệu sản phẩm không hợp lệ");
                if (request.Id != null) request.Id = null;
                if (request.IngredientId == null) throw new ArgumentNullException("Nguyên liệu không hợp lệ");
                var ingredientExists = await _ingredientRepository.GetById(request.IngredientId);
                if (ingredientExists == null) throw new KeyNotFoundException("Nguyên liệu không tồn tại");

                var newProduct = _mapper.Map<IngredientProduct>(request);
                newProduct.TotalPrice = request.Quantity * ingredientExists.PriceOrigin;
                var created = await _ingredientProductRepository.CreateAsync(newProduct);

                // xử lý trừ số quantity
                

                var response = _mapper.Map<IngredientProductResponse>(created);
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
                var ingredientProduct = await _ingredientProductRepository.GetIngredientProductbyId(ingredientProductId);
                return _mapper.Map<IngredientProductResponse>(ingredientProduct);
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
                request.Id = id;
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
