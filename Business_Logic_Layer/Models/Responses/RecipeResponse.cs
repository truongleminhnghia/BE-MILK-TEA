using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
    public class RecipeResponse
    {
        public Guid Id { get; set; }
        public string RecipeTitle { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public RecipeStatusEnum RecipeStatus { get; set; }
        public List<RecipeIngredientResponse> Ingredients { get; set; } = new();
        public CategoryResponse Category { get; set; }

    }

    public class RecipeIngredientResponse
    {
        public Guid Id { get; set; }
        public Guid IngredientId { get; set; }        
        public float WeightOfIngredient { get; set; }
        public Ingredient? Ingredient { get; set; }
    }
}
