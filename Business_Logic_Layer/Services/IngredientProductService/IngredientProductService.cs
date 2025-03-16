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
                bool result = await CheckQuantity(request.Quantity, request.ProductType, ingredient.Id);
                if (!result)
                {
                    throw new Exception("Số lượng không phù hợp");
                }
                var newProduct = _mapper.Map<IngredientProduct>(request);
                if (isCart)
                {
                    newProduct.TotalPrice = 0;
                }
                else
                {
                    if (ingredient.PricePromotion > 0.0 && request.ProductType.Equals(ProductType.Thung))
                    {
                        newProduct.TotalPrice = ingredient.PricePromotion * request.Quantity * ingredient.QuantityPerCarton;
                    }
                    else if (ingredient.PricePromotion > 0.0 && request.ProductType.Equals(ProductType.Bich))
                    {
                        newProduct.TotalPrice = ingredient.PricePromotion * request.Quantity;
                    }
                    else if (ingredient.PricePromotion <= 0.0 && request.ProductType.Equals(ProductType.Bich))
                    {
                        newProduct.TotalPrice = ingredient.PriceOrigin * request.Quantity;
                    }
                    else if (ingredient.PricePromotion <= 0.0 && request.ProductType.Equals(ProductType.Thung))
                    {
                        newProduct.TotalPrice = ingredient.PriceOrigin * request.Quantity * ingredient.QuantityPerCarton;
                    }
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
                if (ingredientProduct != null)
                {
                    var ingredient = await _ingredientRepository.GetById(ingredientProduct.IngredientId);
                    if (ingredient != null)
                    {
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

        public async Task<IngredientProductResponse> UpdateAsync(Guid id, IngredientProductRequest request, bool isCart)
        {
            try
            {
                if (request == null) throw new ArgumentNullException("Dữ liệu cập nhật không hợp lệ.");
                if (id == Guid.Empty) throw new ArgumentNullException("Id không hợp lệ.");
                if (request.IngredientId == Guid.Empty) throw new ArgumentNullException("Nguyên liệu không hợp lệ.");

                // Kiểm tra sản phẩm có tồn tại hay không
                var existingProduct = await _ingredientProductRepository.GetIngredientProductbyId(id);
                if (existingProduct == null)
                {
                    throw new Exception("Sản phẩm không tồn tại.");
                }

                // Kiểm tra nguyên liệu có hợp lệ không
                var ingredient = await _ingredientRepository.GetById(request.IngredientId);
                if (ingredient == null)
                {
                    throw new Exception("Nguyên liệu không tồn tại.");
                }

                // Kiểm tra số lượng có hợp lệ không
                bool isValidQuantity = await CheckQuantity(request.Quantity, request.ProductType, ingredient.Id);
                if (!isValidQuantity)
                {
                    throw new Exception("Số lượng không hợp lệ.");
                }

                // Cập nhật thông tin sản phẩm
                existingProduct.IngredientId = request.IngredientId;
                existingProduct.Quantity = request.Quantity;
                existingProduct.ProductType = request.ProductType;

                // Nếu là cart thì không tính giá
                if (isCart)
                {
                    existingProduct.TotalPrice = 0;
                }
                else
                {
                    existingProduct.TotalPrice = (ingredient.PricePromotion > 0)
                        ? ingredient.PricePromotion * request.Quantity
                        : ingredient.PriceOrigin * request.Quantity;
                }

                // Thực hiện cập nhật vào database
                var updatedProduct = await _ingredientProductRepository.UpdateAsync(existingProduct);

                // Trả về response
                return _mapper.Map<IngredientProductResponse>(updatedProduct);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật IngredientProduct: " + ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    throw new ArgumentNullException("Id không hợp lệ.");

                // Tìm sản phẩm theo ID
                var existingProduct = await _ingredientProductRepository.GetIngredientProductbyId(id);
                if (existingProduct == null)
                    throw new Exception("Sản phẩm không tồn tại.");

                // Xóa sản phẩm khỏi database
                var isDeleted = await _ingredientProductRepository.DeleteAsync(existingProduct);
                if (!isDeleted)
                    throw new Exception("Xóa sản phẩm thất bại.");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa IngredientProduct: " + ex.Message);
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
