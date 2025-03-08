using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Requests
{
    public class RecipeRequest
    {
        public string RecipeTitle { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }

        public List<RecipeIngredientRequest> Ingredients { get; set; } = new();
    }

    public class RecipeIngredientRequest
    {
        public Guid IngredientId { get; set; }

        public float WeightOfIngredient { get; set; }
    }

}
