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

        public async Task<Recipe> CreateRecipe(RecipeRequest request)
        {
            var strategy = _context.Database.CreateExecutionStrategy(); // Lấy chiến lược thực thi an toàn
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
                    var recipe = _mapper.Map<Recipe>(request);
                    recipe.RecipeStatus = RecipeStatusEnum.ACTIVE;
                    await _recipeRepository.CreateRecipe(recipe);
                    // Kiểm tra xem các nguyên liệu có tồn tại không
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
                    return recipe;
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

                return new RecipeResponse
                {
                    Id = recipe.Id,
                    RecipeTitle = recipe.RecipeTitle,
                    Content = recipe.Content,

                    Category = recipe.Category == null ? null : new CategoryResponse
                    {
                        Id = recipe.Category.Id,
                        CategoryName = recipe.Category.CategoryName,
                        CreateAt = recipe.Category.CreateAt,
                        CategoryStatus = recipe.Category.CategoryStatus,
                        CategoryType = recipe.Category.CategoryType
                    },
                    Ingredients = recipe.IngredientRecipes.Select(ir => new IngredientResponse
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
                            CreateAt = ir.Ingredient.Category.CreateAt,
                            CategoryStatus = ir.Ingredient.Category.CategoryStatus,
                            CategoryType = ir.Ingredient.Category.CategoryType
                        },
                        IsSale = ir.Ingredient.IsSale,
                        Rate = ir.Ingredient.Rate,
                        CreateAt = ir.Ingredient.CreateAt,
                        UpdateAt = ir.Ingredient.UpdateAt,
                        IngredientType = ir.Ingredient.IngredientType.ToString(),
                        Images = ir.Ingredient.Images?.Select(img => new ImageRespone
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
                        }).ToList()
                    }).ToList()

                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }

        public async Task<bool> UpdateRecipe(Guid recipeId, RecipeRequest request)
        {
            try
            {
                var recipe = await _recipeRepository.GetRecipeById(recipeId);
                if (recipe == null) throw new Exception("Lấy thông tin thất bại!");

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

                return true; // Return true if update is successful

            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return false; // Return false in case of an error

            }
        }

        public async Task<IEnumerable<RecipeResponse>> GetAllRecipes(
    string? search, string? sortBy, bool isDescending,
    Guid? categoryId, int page, int pageSize)
        {
            try
            {
                var recipes = await _recipeRepository.GetAllRecipes(
                search, sortBy, isDescending, categoryId, page, pageSize);
            
                return recipes.Select(recipe => new RecipeResponse
                {
                    Id = recipe.Id,
                    RecipeTitle = recipe.RecipeTitle,
                    Content = recipe.Content,
                    Category = recipe.Category == null ? null : new CategoryResponse
                    {
                        Id = recipe.Category.Id,
                        CategoryName = recipe.Category.CategoryName,
                        CreateAt = recipe.Category.CreateAt,
                        CategoryStatus = recipe.Category.CategoryStatus,
                        CategoryType = recipe.Category.CategoryType
                    },

                    Ingredients = recipe.IngredientRecipes.Select(ir => new IngredientResponse
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
                            CreateAt = ir.Ingredient.Category.CreateAt,
                            CategoryStatus = ir.Ingredient.Category.CategoryStatus,
                            CategoryType = ir.Ingredient.Category.CategoryType
                        },
                        IsSale = ir.Ingredient.IsSale,
                        Rate = ir.Ingredient.Rate,
                        CreateAt = ir.Ingredient.CreateAt,
                        UpdateAt = ir.Ingredient.UpdateAt,
                        IngredientType = ir.Ingredient.IngredientType.ToString(),
                        Images = ir.Ingredient.Images?.Select(img => new ImageRespone
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
                        }).ToList()
                    }).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
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

            return new PageResult<RecipeResponse>
            {
                Data = _mapper.Map<List<RecipeResponse>>(recipes),
                PageCurrent = page,
                PageSize = pageSize,
                Total = total
            };
        }

    }
}
