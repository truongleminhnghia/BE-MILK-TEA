using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Utils;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services.IngredientService
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _repository;
        private readonly Source _source;

        public IngredientService(IIngredientRepository repository, Source source)
        {
            _repository = repository;
            _source = source;
        }

        public async Task<IEnumerable<Ingredient>> GetAllIngredientsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Ingredient> GetIngredientByIdAsync(Guid id)
        {
            var ingredient = await _repository.GetByIdAsync(id);
            if (ingredient == null)
            {
                throw new KeyNotFoundException("Ingredient not found");
            }
            return ingredient;
        }

        public async Task<Ingredient> CreateIngredientAsync(Ingredient ingredient)
        {
            // Check if the CategoryId exists
            var categoryExists = await _repository.CategoryExistsAsync(ingredient.CategoryId);
            if (!categoryExists)
            {
                throw new ArgumentException("Invalid CategoryId");
            }

            // Generate ingredient code (PCxxxxx)
            ingredient.IngredientCode = await GenerateIngredientCode();
            return await _repository.CreateAsync(ingredient);

            // try catch

            var role = _source.CheckRoleName();
            if(!role.Equals(RoleName.ROLE_ADMIN.ToString())) {
                throw new Exception("401, bạn không có quyền");
            }
            string code = "";
            code = "PC" + _source.GenerateRandom8Digits();
            if(_repository.FindCode(code) != null) {
                throw new Exception("Code already exist");
            }            

            

        }

        public async Task<Ingredient> UpdateIngredientAsync(Guid id, Ingredient ingredient)
        {
            var existingIngredient = await _repository.GetByIdAsync(id);
            if (existingIngredient == null)
                throw new DirectoryNotFoundException("Ingredient not found");

            // Check if the CategoryId exists
            var categoryExists = await _repository.CategoryExistsAsync(ingredient.CategoryId);
            if (!categoryExists)
            {
                throw new ArgumentException("Invalid CategoryId");
            }

            ingredient.Id = id;
            ingredient.IngredientCode = existingIngredient.IngredientCode; // Preserve original code
            return await _repository.UpdateAsync(ingredient);
        }

        public async Task<bool> DeleteIngredientAsync(Guid id)
        {
            return await _repository.DeleteAsync(id);
        }

        private async Task<string> GenerateIngredientCode()
        {
            var lastIngredient = await _repository.GetLastIngredientCode();
            int newNumber = 1;

            if (lastIngredient != null && lastIngredient.IngredientCode.StartsWith("PC"))
            {
                string numberPart = lastIngredient.IngredientCode.Substring(2);
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    newNumber = lastNumber + 1;
                }
            }

            return $"PC{newNumber:D5}";
        }
    }
}
