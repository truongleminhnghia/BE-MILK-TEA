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
            var recipe = await _recipeRepository.GetRecipeById(recipeId);
            if (recipe == null) return null;

            // Cập nhật thông tin
            recipe.RecipeTitle = request.RecipeTitle;
            recipe.Content = request.Content;
            recipe.CategoryId = request.CategoryId;

            // Xóa danh sách nguyên liệu cũ và thêm mới
            recipe.IngredientRecipes.Clear();
            foreach (var ingredient in request.Ingredients)
            {
                recipe.IngredientRecipes.Add(new IngredientRecipe
                {
                    RecipeId = recipe.Id,
                    IngredientId = ingredient.IngredientId,
                    WeightOfIngredient = ingredient.WeightOfIngredient
                });
            }

            var isUpdated = await _recipeRepository.UpdateAsync(recipe);
            if (!isUpdated) return null;

            // Chuyển đổi sang RecipeResponse
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

    }
}
