using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories.Entities
{
    [Table("ingredient_recipe")]
    public class IngredientRecipe
    {
        [Key]
        [Column("ingredient_recipe_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("ingredient_id")]
        [Required]
        [ForeignKey("ingredient_id")]
        public Guid IngredientId { get; set; }

        public Ingredient? Ingredient { get; set; }

        [Column("recipe_id")]
        [Required]
        [ForeignKey("recipe_id")]
        public Guid RecipeId { get; set; }

        public Recipe? Recipe { get; set; }
    }
}