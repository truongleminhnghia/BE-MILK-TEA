using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    public class Recipe
    {
        [Key]
        [Column ("recipe_id")]
        public Guid RecipeId { get; set; }

        [Column ("recipe_code", TypeName = "nvarchar(300)")]
        public string RecipeCode { get; set; }
        
        [Column ("recipe_title", TypeName = "nvarchar(300)")]
        public string RecipeTitle { get; set; }

        [Column ("content", TypeName = "nvarchar")]
        [MaxLength (1000)]
        public string Content { get; set; }

        [Column("category_id")]
        public Guid CategoryId { get; set; }

        //      setup relationship
        public virtual Category Category { get; set; }
        public virtual ICollection<IngredientRecipe> IngredientsRecipes { get; set; }

        //     constructor
        public Recipe(string recipeCode, string recipeTitle, string content, Guid categoryId)
        {
            RecipeId = Guid.NewGuid();
            RecipeCode = recipeCode;
            RecipeTitle = recipeTitle;
            Content = content;
            CategoryId = categoryId;
        }

    }
}
