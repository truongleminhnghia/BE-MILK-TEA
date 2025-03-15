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
                    return _mapper.Map<RecipeResponse>(recipe);
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
                if (recipe == null)
                {
                    throw new Exception("Không tìm thấy công thức!");
                }

                return _mapper.Map<RecipeResponse>(recipe);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy công thức: {ex.Message}");
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
                return _mapper.Map<RecipeResponse>(recipe);
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

            return new PageResult<RecipeResponse>
            {
                Data = _mapper.Map<List<RecipeResponse>>(recipes),
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
                return _mapper.Map<RecipeResponse>(recipe);
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
        /*
        private RecipeResponse ComplexRecipeResponse(Recipe recipe)
        {
            if (recipe == null)
                throw new ArgumentNullException(nameof(recipe), "Recipe không thể là null!");

            return new RecipeResponse
            {
                Id = recipe.Id,
                RecipeTitle = recipe.RecipeTitle,
                Content = recipe.Content,
                Category = recipe.Category != null ? new Category
                {
                    Id = recipe.Category.Id,
                    CategoryName = recipe.Category.CategoryName,
                    CreateAt = recipe.Category.CreateAt ?? DateTime.MinValue, // Tránh ép kiểu null
                    CategoryStatus = recipe.Category.CategoryStatus,
                    CategoryType = recipe.Category.CategoryType,
                } : null, // Tránh lỗi nếu Category null
                ImageUrl = recipe.ImageUrl,
                RecipeStatus = recipe.RecipeStatus,
                Ingredients = (recipe.IngredientRecipes ?? new List<IngredientRecipe>()).Select(ir => new RecipeIngredientResponse
                {
                    Id = ir.Id,
                    IngredientId = ir.IngredientId,
                    WeightOfIngredient = ir.WeightOfIngredient,
                    Ingredient = ir.Ingredient != null ? new Ingredient
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
                        IngredientType = ir.Ingredient.IngredientType,
                        Unit = ir.Ingredient.Unit,
                        PriceOrigin = ir.Ingredient.PriceOrigin,
                        PricePromotion = ir.Ingredient.PricePromotion,
                        CategoryId = ir.Ingredient.CategoryId,
                        Category = ir.Ingredient.Category != null ? new Category
                        {
                            Id = ir.Ingredient.CategoryId,
                            CategoryName = ir.Ingredient.Category.CategoryName,
                            CreateAt = ir.Ingredient.Category.CreateAt ?? DateTime.MinValue, // Tránh ép kiểu null
                            CategoryStatus = ir.Ingredient.Category.CategoryStatus,
                            CategoryType = ir.Ingredient.Category.CategoryType,
                        } : null, // Tránh lỗi nếu Category null
                        IsSale = ir.Ingredient.IsSale,
                        Rate = ir.Ingredient.Rate,
                        CreateAt = ir.Ingredient.CreateAt ?? DateTime.MinValue, // Tránh ép kiểu null
                        UpdateAt = ir.Ingredient.UpdateAt ?? DateTime.MinValue
                    } : null // Tránh lỗi nếu Ingredient null
                }).ToList()
            };
        }
        */
    }
}
