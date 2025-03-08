using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;

        public RecipeService(IRecipeRepository recipeRepository, IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
        }

        public async Task<Recipe> CreateRecipe(RecipeRequest request)
        {
            try
            {
                var recipe = _mapper.Map<Recipe>(request);

                await _recipeRepository.CreateRecipe(recipe);
                // Kiểm tra xem các nguyên liệu có tồn tại không
                foreach (var ingredientRequest in request.Ingredients)
                {
                    var ingredient = await _ingredientRepository.GetByIdAsync(ingredientRequest.IngredientId);
                    if (ingredient == null)
                    {
                        throw new ArgumentException($"Nguyên liệu với ID {ingredientRequest.IngredientId} không tồn tại.");
                    }

                    var ingredientRecipe = new IngredientRecipe
                    {
                        RecipeId = recipe.Id,
                        IngredientId = ingredientRequest.IngredientId,
                        WeightOfIngredient = ingredientRequest.WeightOfIngredient
                    };

                    recipe.IngredientRecipes.Add(ingredientRecipe);
                }

                return recipe;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }

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
                    CategoryId = recipe.CategoryId,
                    Ingredients = recipe.IngredientRecipes.Select(ir => new RecipeIngredientResponse
                    {
                        IngredientId = ir.Ingredient.Id,
                        IngredientName = ir.Ingredient.IngredientName,
                        WeightOfIngredient = ir.WeightOfIngredient
                    }).ToList()
                };
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
                return new RecipeResponse
                {
                    Id = recipe.Id,
                    RecipeTitle = recipe.RecipeTitle,
                    Content = recipe.Content,
                    CategoryId = recipe.CategoryId,
                    Ingredients = recipe.IngredientRecipes.Select(ir => new RecipeIngredientResponse
                    {
                        IngredientId = ir.IngredientId,
                        IngredientName = ir.Ingredient.IngredientName,
                        WeightOfIngredient = ir.WeightOfIngredient
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
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
                    CategoryId = recipe.CategoryId,
                    CategoryName = recipe.Category?.CategoryName,
                    Ingredients = recipe.IngredientRecipes.Select(ir => new RecipeIngredientResponse
                    {
                        IngredientId = ir.IngredientId,
                        IngredientName = ir.Ingredient.IngredientName,
                        WeightOfIngredient = ir.WeightOfIngredient
                    }).ToList()
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }

    }
}
