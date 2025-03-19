using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Data_Access_Layer.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Business_Logic_Layer.Models;

namespace Business_Logic_Layer.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly IIngredientRecipeRepository _ingredientRecipeRepository;
        private readonly ICategoryService _categoryService;

        public RecipeService(IRecipeRepository recipeRepository, IIngredientRepository ingredientRepository, IMapper mapper, ApplicationDbContext applicationDbContext, IIngredientRecipeRepository ingredientRecipeRepository, ICategoryService categoryService)
        {
            _recipeRepository = recipeRepository;
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
            _context = applicationDbContext;
            _ingredientRecipeRepository = ingredientRecipeRepository;
            _categoryService = categoryService;
        }

        public async Task<RecipeResponse> CreateRecipe(RecipeRequest request)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var checkCate = await _categoryService.GetByIdAsync(request.CategoryId);
                    if (checkCate.CategoryType != CategoryType.CATEGORY_RECIPE)
                    {
                        throw new Exception("Danh mục không phải là danh mục công thức.");
                    }

                    // Kiểm tra dữ liệu đầu vào
                    if (!IsValidText(request.RecipeTitle))
                    {
                        throw new Exception("Tên công thức chứa ký tự không hợp lệ.");
                    }

                    // Chuẩn hóa dữ liệu
                    request.RecipeTitle = NormalizeText(request.RecipeTitle);
                    var checkRepcipeTitle = await _recipeRepository.GetByTitleAsync(request.RecipeTitle);
                    if (checkRepcipeTitle != null)
                    {
                        throw new Exception("Tên công thức đã tồn tại.");
                    }
                    request.Content = string.IsNullOrWhiteSpace(request.Content) ? null : NormalizeText(request.Content);

                    var recipe = _mapper.Map<Recipe>(request);
                    recipe.RecipeStatus = RecipeStatusEnum.INACTIVE;

                    await _recipeRepository.CreateRecipe(recipe);

                    // Kiểm tra nguyên liệu
                    foreach (var ingredientRequest in request.Ingredients)
                    {
                        var ingredient = await _ingredientRepository.GetById(ingredientRequest.IngredientId);
                        if (ingredient == null)
                        {
                            throw new ArgumentException($"Nguyên liệu với ID {ingredientRequest.IngredientId} không tồn tại.");
                        }

                        var ingreRecipe = _mapper.Map<IngredientRecipe>(ingredientRequest);
                        ingreRecipe.RecipeId = recipe.Id;
                        await _ingredientRecipeRepository.CreateIngredientRecipe(ingreRecipe);
                    }

                    await transaction.CommitAsync();
                    return ComplexRecipeResponse(recipe);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error: " + ex.Message);
                    await transaction.RollbackAsync();
                    throw new Exception("Error: " + ex.Message);
                }
            });
        }


        public async Task<RecipeResponse?> GetRecipeById(Guid recipeId)
        {
            try
            {
                var recipe = await _recipeRepository.GetRecipeById(recipeId);
                if (recipe == null) throw new Exception("Lấy thông tin thất bại!");

                return ComplexRecipeResponse(recipe);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }


        public async Task<RecipeResponse?> UpdateRecipe(Guid recipeId, RecipeRequest request)
        {
            try
            {
                var recipe = await _recipeRepository.GetRecipeById(recipeId);
                if (recipe == null) throw new Exception("Lấy thông tin thất bại!");

                // Kiểm tra dữ liệu đầu vào
                if (!IsValidText(request.RecipeTitle))
                {
                    throw new Exception("Tên công thức chứa ký tự không hợp lệ.");
                }

                // Chuẩn hóa dữ liệu
                request.RecipeTitle = NormalizeText(request.RecipeTitle);
                var checkRepcipeTitle = await _recipeRepository.GetByTitleAsync(request.RecipeTitle);
                if (checkRepcipeTitle != null)
                {
                    throw new Exception("Tên công thức đã tồn tại.");
                }
                request.Content = string.IsNullOrWhiteSpace(request.Content) ? null : NormalizeText(request.Content);

                // Cập nhật thông tin công thức
                recipe.RecipeTitle = request.RecipeTitle;
                recipe.Content = request.Content;
                recipe.CategoryId = request.CategoryId;

                // Xóa tất cả nguyên liệu cũ trước khi thêm mới
                await _recipeRepository.DeleteIngredientsByRecipeIdAsync(recipeId);

                // Thêm nguyên liệu mới
                recipe.IngredientRecipes = request.Ingredients.Select(ingredient => new IngredientRecipe
                {
                    RecipeId = recipe.Id,
                    IngredientId = ingredient.IngredientId,
                    WeightOfIngredient = ingredient.WeightOfIngredient
                }).ToList();

                await _recipeRepository.UpdateRecipe(recipe);

                // Trả về DTO
                return ComplexRecipeResponse(recipe);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                throw new Exception("Có lỗi khi cập nhật Recipe: " + ex.Message); // Return false in case of an error

            }
        }

        public async Task<PageResult<RecipeResponse>> GetAllRecipesAsync(
    string? search, string? sortBy, bool isDescending,
    RecipeStatusEnum? recipeStatus, Guid? categoryId,
    DateTime? startDate, DateTime? endDate,
    int page, int pageSize)
        {
            var (recipes, total) = await _recipeRepository.GetAllRecipesAsync(
                search, sortBy, isDescending, recipeStatus, categoryId, startDate, endDate, page, pageSize);
            if (recipes == null)
            {
                throw new Exception("Recipe repository returned null.");
            }

            return new PageResult<RecipeResponse>
            {
                Data = recipes.Select(ComplexRecipeResponse).ToList(),
                PageCurrent = page,
                PageSize = pageSize,
                Total = total
            };
        }


        public async Task<RecipeResponse?> UpdateRecipeStatusAsync(Guid recipeId, RecipeStatusEnum newStatus)
        {
            try
            {
                // Lấy thông tin công thức theo ID
                var recipe = await _recipeRepository.GetRecipeById(recipeId);
                if (recipe == null) throw new Exception("Không tìm thấy công thức!");

                // Kiểm tra trạng thái hợp lệ
                if (!Enum.IsDefined(typeof(RecipeStatusEnum), newStatus))
                {
                    throw new Exception("Trạng thái không hợp lệ!");
                }

                // Cập nhật trạng thái công thức
                recipe.RecipeStatus = newStatus;
                await _recipeRepository.UpdateRecipe(recipe);

                // Trả về Response
                return ComplexRecipeResponse(recipe);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }


        // Hàm chuẩn hóa dữ liệu (xoá khoảng trắng thừa, không có ký tự đặc biệt)
        private string NormalizeText(string input)
        {
            input = input.Trim();
            input = Regex.Replace(input, @"\s+", " "); // Xóa khoảng trắng thừa giữa các từ
            return input;
        }

        // Kiểm tra xem chuỗi có chứa ký tự đặc biệt không
        private bool IsValidText(string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z0-9À-Ỷà-ỷ\s]+$"); // Chỉ cho phép chữ cái, số và khoảng trắng
        }

        private RecipeResponse ComplexRecipeResponse(Recipe recipe)
        {
            if (recipe == null)
                throw new ArgumentNullException(nameof(recipe), "Recipe không thể bỏ trống!");

            return new RecipeResponse
            {
                Id = recipe.Id,
                RecipeTitle = recipe.RecipeTitle,
                Content = recipe.Content,

                Category = recipe.Category == null ? null : new CategoryResponse
                {
                    Id = recipe.Category.Id,
                    CategoryName = recipe.Category.CategoryName,
                    CreateAt = (DateTime)recipe.Category.CreateAt,
                    CategoryStatus = recipe.Category.CategoryStatus,
                    CategoryType = recipe.Category.CategoryType
                },
                Ingredients = recipe.IngredientRecipes?.Select(ir => new IngredientResponse
                {
                    Id = ir.Ingredient.Id,
                    IngredientCode = ir.Ingredient.IngredientCode,
                    Supplier = ir.Ingredient.Supplier,
                    IngredientName = ir.Ingredient.IngredientName,
                    Description = ir.Ingredient.Description,
                    FoodSafetyCertification = ir.Ingredient.FoodSafetyCertification,
                    ExpiredDate = ir.Ingredient.ExpiredDate,
                    IngredientStatus = ir.Ingredient.IngredientStatus,
                    WeightPerBag = ir.Ingredient.WeightPerBag,
                    QuantityPerCarton = ir.Ingredient.QuantityPerCarton,
                    Unit = ir.Ingredient.Unit,
                    PriceOrigin = ir.Ingredient.PriceOrigin,
                    PricePromotion = ir.Ingredient.PricePromotion,
                    Category = ir.Ingredient.Category == null ? null : new CategoryResponse
                    {
                        Id = ir.Ingredient.Category.Id,
                        CategoryName = ir.Ingredient.Category.CategoryName,
                        CreateAt = (DateTime)ir.Ingredient.Category.CreateAt,
                        CategoryStatus = ir.Ingredient.Category.CategoryStatus,
                        CategoryType = ir.Ingredient.Category.CategoryType
                    },
                    IsSale = ir.Ingredient.IsSale,
                    Rate = ir.Ingredient.Rate,
                    CreateAt = (DateTime)ir.Ingredient.CreateAt,
                    UpdateAt = (DateTime)ir.Ingredient.UpdateAt,
                    IngredientType = ir.Ingredient.IngredientType.ToString(),
                    Images = ir.Ingredient.Images?.Select(img => new ImageResponse
                    {
                        Id = img.Id,
                        ImageUrl = img.ImageUrl,
                        IngredientId = img.IngredientId
                    }).ToList(),
                    IngredientQuantities = ir.Ingredient.IngredientQuantities?.Select(iq => new IngredientQuantityResponse
                    {
                        Id = iq.Id,
                        IngredientId = iq.IngredientId,
                        Quantity = iq.Quantity,
                        ProductType = iq.ProductType
                    }).ToList(),
                }).ToList(),
                IngredientRecipeResponse = recipe.IngredientRecipes?.Select(ir => new RecipeIngredientResponse
                {
                    Id = ir.Id,
                    IngredientId = ir.Ingredient.Id,
                    IngredientName = ir.Ingredient.IngredientName,
                    WeightOfIngredient = ir.WeightOfIngredient
                }).ToList() ?? new List<RecipeIngredientResponse>()
            };
        }

    }
}
