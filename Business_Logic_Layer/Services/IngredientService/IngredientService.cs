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
using Data_Access_Layer.Data;
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
        private readonly ApplicationDbContext _context;

        public IngredientService(IIngredientRepository ingredientRepository, IMapper mapper, ICategoryService categoryService, Source source, IImageService imageService, IIngredientQuantityService ingredientQuantityService, ApplicationDbContext context)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
            _categoryService = categoryService;
            _source = source;
            _imageSerivce = imageService;
            _ingredientQuantityService = ingredientQuantityService;
            _context = context;
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
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
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

                    var minWeight = new Dictionary<UnitOfIngredientEnum, double>
                    {
                        { UnitOfIngredientEnum.KG, 1 },
                        { UnitOfIngredientEnum.GRAM, 1 },
                        { UnitOfIngredientEnum.ML, 1 },
                        { UnitOfIngredientEnum.L, 1 }
                    };
                    if (request.WeightPerBag < minWeight[request.Unit])
                    {
                        throw new Exception($"Khối lượng trong mỗi túi phải lớn hơn {minWeight[request.Unit]} {request.Unit}.");
                    }

                    var ingredient = _mapper.Map<Ingredient>(request);
                    ingredient.IngredientCode = "P" + _source.GenerateRandom8Digits();
                    ingredient.Category = categoryExists;
                    ingredient.CreateAt = DateTime.Now;
                    if (await _ingredientRepository.CheckCode(ingredient.IngredientCode))
                    {
                        throw new Exception("Mã nguyên liệu đã tồn tại");
                    }
                    int checkDate = _source.CheckDate(ingredient.ExpiredDate);
                    if (checkDate == 1 || checkDate == -1)
                    {
                        throw new Exception("Hạn sử dụng chỉ còn 10 ngày hoặc hết hạn");
                    }
                    IngredientResponse ingredientResponse = _mapper.Map<IngredientResponse>(await _ingredientRepository.CreateAsync(ingredient));

                    if (ingredientResponse == null)
                    {
                        throw new Exception("Tạo nguyên liệu mới không thành công");
                    }

                    List<ImageResponse> imageRespones = await _imageSerivce.AddImages(ingredientResponse.Id, request.ImageRequest);

                    List<IngredientQuantityResponse> ingredientQuantities = await _ingredientQuantityService.CreateQuantitiesAsync(ingredientResponse.Id, request.IngredientQuantities);

                    await transaction.CommitAsync();
                    ingredient.Images = _mapper.Map<List<Image>>(imageRespones);
                    ingredientResponse.Category = _mapper.Map<CategoryResponse>(categoryExists);
                    ingredientResponse.Images = imageRespones;
                    return ingredientResponse;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error: " + ex.Message);
                    await transaction.RollbackAsync();
                    throw new Exception("Error: " + ex.Message);
                }
            });
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
                throw;
            }
        }

        public async Task<IngredientResponse> Update(Guid id, UpdateIngredientRequest request)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Fetch existing ingredient
                    Ingredient ingredient = await _ingredientRepository.GetById(id);
                    if (ingredient == null)
                    {
                        throw new Exception("Nguyên liệu không tồn tại");
                    }

                    // Validate category if provided
                    if (request.CategoryId.HasValue)
                    {
                        var categoryExists = await _categoryService.GetByIdAsync(request.CategoryId.Value);
                        if (categoryExists == null)
                        {
                            throw new Exception("Danh mục không tồn tại");
                        }
                        if (categoryExists.CategoryType != CategoryType.CATEGORY_PRODUCT)
                        {
                            throw new Exception("Danh mục không phải là danh mục nguyên liệu");
                        }
                        ingredient.Category = categoryExists;
                    }

                    // Validate weight per bag
                    if (request.WeightPerBag.HasValue)
                    {
                        var minWeight = new Dictionary<UnitOfIngredientEnum, double>
                {
                    { UnitOfIngredientEnum.KG, 1 },
                    { UnitOfIngredientEnum.GRAM, 1 },
                    { UnitOfIngredientEnum.ML, 1 },
                    { UnitOfIngredientEnum.L, 1 }
                };

                        // Use new unit if provided, otherwise use existing ingredient's unit
                        var unit = request.Unit ?? ingredient.Unit;
                        if (request.WeightPerBag.Value < minWeight[unit])
                        {
                            throw new Exception($"Khối lượng trong mỗi túi phải lớn hơn {minWeight[unit]} {unit}.");
                        }
                    }

                    // Validate expired date if provided
                    if (request.ExpiredDate.HasValue)
                    {
                        int checkDate = _source.CheckDate(request.ExpiredDate.Value);
                        if (checkDate == 1 || checkDate == -1)
                        {
                            throw new Exception("Hạn sử dụng chỉ còn 10 ngày hoặc hết hạn");
                        }
                    }

                    // Validate image request
                    if (request.ImageRequest != null && !request.ImageRequest.Any())
                    {
                        throw new Exception("Danh sách ảnh trống");
                    }

                    // Use mapper to merge request with existing ingredient
                    var ingredientToUpdate = _mapper.Map(request, ingredient);

                    // Perform update
                    var updatedIngredient = await _ingredientRepository.UpdateAsync(id, ingredientToUpdate);
                    if (updatedIngredient == null)
                    {
                        throw new Exception("Cập nhật thất bại");
                    }

                    // Map to response
                    var res = _mapper.Map<IngredientResponse>(updatedIngredient);

                    // Handle image updates
                    if (request.ImageRequest != null)
                    {
                        var imageRes = await _imageSerivce.UpdateImages(request.ImageRequest, updatedIngredient.Id);
                        if (imageRes != null) res.Images = imageRes;
                    }

                    // Handle ingredient quantity updates
                    if (request.IngredientQuantities != null)
                    {
                        var quantityRes = await _ingredientQuantityService.UpdateQuantitiesAsync(updatedIngredient.Id, request.IngredientQuantities);
                        if (quantityRes != null) res.IngredientQuantities = quantityRes;
                    }

                    // Commit transaction
                    await transaction.CommitAsync();

                    return res;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    await transaction.RollbackAsync();
                    throw new Exception("Error: " + ex.Message);
                }
            });
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

        public async Task<IngredientResponse> GetByIdOrCode(Guid? id, string? code)
        {
            if (id.HasValue)
            {
                return await GetById(id.Value);
            }
            if (!string.IsNullOrEmpty(code))
            {
                var ingredient = await _ingredientRepository.GetByIdOrCode(id, code);
                if (ingredient == null)
                {
                    throw new KeyNotFoundException("Ingredient not found");
                }
                return _mapper.Map<IngredientResponse>(ingredient);
            }
            return null;
        }
    }
}
