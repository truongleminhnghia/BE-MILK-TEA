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

        public string ImageUrl { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }

        public List<RecipeIngredientRequest> Ingredients { get; set; } = new();
    }

    public class RecipeIngredientRequest
    {
        public Guid IngredientId { get; set; }
        [Range(0.1, double.MaxValue, ErrorMessage = "Khối lượng của nguyên liệu phải lớn hơn 0")]
        public float WeightOfIngredient { get; set; }
    }

}
