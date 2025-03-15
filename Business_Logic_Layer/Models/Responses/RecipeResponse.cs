using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
    public class RecipeResponse
    {
        public Guid Id { get; set; }
        public string RecipeTitle { get; set; }
        public string Content { get; set; }
        public CategoryResponse Category { get; set; }
        public List<IngredientResponse> Ingredients { get; set; } = new();
    }

    public class RecipeIngredientResponse
    {
        public Guid Id { get; set; }
        public Guid IngredientId { get; set; }
        public string IngredientName { get; set; }
        public float WeightOfIngredient { get; set; }
    }
}
