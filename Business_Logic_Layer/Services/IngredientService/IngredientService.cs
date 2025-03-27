using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Utils;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic_Layer.Services.IngredientService
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;
        private readonly Source _source;
        private readonly IImageService _imageSerivce;
        private readonly IIngredientQuantityService _ingredientQuantityService;

        public IngredientService(IIngredientRepository ingredientRepository, IMapper mapper, ICategoryService categoryService, Source source, IImageService imageService, IIngredientQuantityService ingredientQuantityService)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
            _categoryService = categoryService;
            _source = source;
            _imageSerivce = imageService;
            _ingredientQuantityService = ingredientQuantityService;
        }

        public async Task<bool> ChangeStatus(Guid id)
        {
            try
            {
                var result = await _ingredientRepository.ChangeStatus(id);
                if (!result)
                {
                    throw new Exception("Thay đổi trạng thái thất bại");
                }
                return result;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return false;
            }
        }

        public async Task<IngredientResponse> CreateIngredientAsync(IngredientRequest request)
        {
            try
            {
                var categoryExists = await _categoryService.GetByIdAsync(request.CategoryId);
                if (categoryExists == null)
                {
                    throw new Exception("Danh mục không tồn tại");
                }
                if (categoryExists.CategoryType != CategoryType.CATEGORY_PRODUCT)
                {
                    throw new Exception("Danh mục không phải là danh mục nguyên liệu");
                }
                if (request.ImageRequest == null || !request.ImageRequest.Any())
                {
                    throw new Exception("Danh sách ảnh trống");
                }
                if (request.Unit == UnitOfIngredientEnum.KG && request.WeightPerBag < 0)
                {
                    throw new Exception("Khối lượng trong mỗi túi tính theo kilogram phải lớn hơn 0");
                }
                else if (request.Unit == UnitOfIngredientEnum.GRAM && request.WeightPerBag < 1)
                {
                    throw new Exception("Khối lượng trong mỗi túi tính theo gram phải lớn hơn 1");
                }
                var ingredient = _mapper.Map<Ingredient>(request);
                ingredient.IngredientCode = "P" + _source.GenerateRandom8Digits();
                ingredient.Category = categoryExists;
                ingredient.CreateAt = DateTime.Now;
                if (await _ingredientRepository.CheckCode(ingredient.IngredientCode))
                {
                    throw new Exception("Mã nguyên liệu đã tồn tại");
                }
                if (_source.CheckDate(ingredient.ExpiredDate) == 1 || _source.CheckDate(ingredient.ExpiredDate) == -1)
                {
                    throw new Exception("Hạn sử dụng chỉ còn 10 ngày hoặc hết hạn");
                }
                IngredientResponse ingredientResponse = _mapper.Map<IngredientResponse>(await _ingredientRepository.CreateAsync(ingredient));

                if (ingredientResponse == null)
                {
                    throw new Exception("Tạo nguyên liệu mới không thành công");
                }

                List<ImageResponse> imageRespones = await _imageSerivce.AddImages(ingredientResponse.Id, request.ImageRequest);

                // List<IngredientQuantityResponse> ingredientQuantities = await _ingredientQuantityService.CreateQuantitiesAsync(ingredientResponse.Id, request.IngredientQuantities);

                ingredient.Images = _mapper.Map<List<Image>>(imageRespones);
                ingredientResponse.Category = _mapper.Map<CategoryResponse>(categoryExists);
                ingredientResponse.Images = imageRespones;
                return ingredientResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return null;
            }
        }

        public async Task<IngredientResponse> GetById(Guid id)
        {
            try
            {
                var ingredient = await _ingredientRepository.GetById(id);
                if (ingredient == null)
                {
                    throw new KeyNotFoundException("Ingredient not found");
                }
                return _mapper.Map<IngredientResponse>(ingredient);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return null;
            }
        }

        public async Task<IngredientResponse> Update(Guid id, UpdateIngredientRequest request)
        {
            try
            {
                Ingredient ingredient = await _ingredientRepository.GetById(id);
                if (ingredient == null)
                {
                    throw new Exception("Nguyên liệu không tồn tại");
                }
                if (request.Supplier != null)
                {
                    ingredient.Supplier = request.Supplier;
                }
                if (request.IngredientName != null)
                {
                    ingredient.IngredientName = request.IngredientName;
                }
                if (request.Description != null)
                {
                    ingredient.Description = request.Description;
                }
                if (request.FoodSafetyCertification != null)
                {
                    ingredient.FoodSafetyCertification = request.FoodSafetyCertification;
                }
                if (request.IngredientStatus != null)
                {
                    ingredient.IngredientStatus = request.IngredientStatus;
                }
                if (request.WeightPerBag.HasValue && request.WeightPerBag.Value > 0f)
                {
                    ingredient.WeightPerBag = request.WeightPerBag.Value;
                }
                if (request.QuantityPerCarton.HasValue && request.QuantityPerCarton > 0)
                {
                    ingredient.QuantityPerCarton = request.QuantityPerCarton.Value;
                }
                if (request.PriceOrigin.HasValue && request.PriceOrigin.Value > 0.0)
                {
                    ingredient.PriceOrigin = request.PriceOrigin.Value;
                }
                if (request.IsSale.HasValue)
                {
                    ingredient.IsSale = request.IsSale.Value;
                }
                var i = await _ingredientRepository.UpdateAsync(id, ingredient);
                var res = _mapper.Map<IngredientResponse>(i);
                if (res == null)
                {
                    throw new Exception("Cập nhật thất bại");
                }
                if (request.ImageRequest != null)
                {
                    var imageRes = await _imageSerivce.UpdateImages(request.ImageRequest, ingredient.Id);
                    if (imageRes != null) res.Images = imageRes;
                }
                if (request.IngredientQuantities != null)
                {
                    var quantityRes = await _ingredientQuantityService.UpdateQuantitiesAsync(ingredient.Id, request.IngredientQuantities);
                    if (quantityRes != null) res.IngredientQuantities = quantityRes;
                }
                return res;                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return null;
            }
        }

        public async Task<PageResult<IngredientResponse>> GetAllAsync(
        string? search,
        string? categorySearch,
        Guid? categoryId,
        string? sortBy,
        bool isDescending,
        int pageCurrent,
        int pageSize,
        DateOnly? startDate,
        DateOnly? endDate,
        IngredientStatus? status,
        decimal? minPrice,
        decimal? maxPrice,
        bool? isSale,
        IngredientType? ingredientType)
        {
            var query = _ingredientRepository.GetAll(search, categorySearch, categoryId, startDate, endDate, status, minPrice, maxPrice, isSale, ingredientType);

            // **Sắp xếp**
            var validSortColumns = new HashSet<string> { "IngredientName", "CreateAt", "PriceOrigin", "CategoryName" };
            if (!string.IsNullOrEmpty(sortBy) && validSortColumns.Contains(sortBy))
            {
                query = isDescending
                    ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                    : query.OrderBy(e => EF.Property<object>(e, sortBy));
            }
            else
            {
                query = query.OrderByDescending(i => i.CreateAt);
            }

            // **Tổng số bản ghi**
            int total = await query.CountAsync();

            // **Phân trang**
            var items = await query
                .Skip((pageCurrent - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResult<IngredientResponse>
            {
                Data = _mapper.Map<List<IngredientResponse>>(items),
                PageCurrent = pageCurrent,
                PageSize = pageSize,
                Total = total
            };
        }
    }
}
