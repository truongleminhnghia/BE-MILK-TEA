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
        public Guid CategoryId { get; set; }
        public List<RecipeIngredientResponse> Ingredients { get; set; } = new();
    }

    public class RecipeIngredientResponse
    {
        public Guid IngredientId { get; set; }
        public string IngredientName { get; set; }
        public float WeightOfIngredient { get; set; }
    }
}
