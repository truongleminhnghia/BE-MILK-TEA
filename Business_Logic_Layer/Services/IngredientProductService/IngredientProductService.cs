using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;


namespace Business_Logic_Layer.Services.IngredientProductService
{
    public class IngredientProductService : IIngredientProductService
    {
        private readonly IIngredientProductRepository _ingredientProductRepository;
        private readonly IMapper _mapper;

        public IngredientProductService(IIngredientProductRepository ingredientProductRepository, IMapper mapper)
        {
            _ingredientProductRepository = ingredientProductRepository;
            _mapper = mapper;
        }

        public async Task<IngredientProductResponse> CreateAsync(IngredientProductRequest request)
        {
            try
            {
                if(request == null) throw new ArgumentNullException("Dữ liệu sản phẩm không hợp lệ");
                if (request.Id != null) request.Id = null;
                var newProduct = _mapper.Map<IngredientProduct>(request);
                var created = await _ingredientProductRepository.CreateAsync(newProduct);
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
                if (ingredientProduct == null)
                {
                    throw new Exception("Không tìm thấy IngredientProduct");
                }
                return _mapper.Map<IngredientProductResponse>(ingredientProduct);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy IngredientProduct: " + ex.Message);
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
                request.Id = id;
                _mapper.Map(request, ingredientProduct);
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
    }
}
