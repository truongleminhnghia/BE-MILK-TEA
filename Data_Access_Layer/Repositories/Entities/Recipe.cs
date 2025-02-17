using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories.Entities
{
    [Table("recipe")]
    public class Recipe : BaseEntity
    {

        [Key]
        [Column("recipe_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("recipe_title", TypeName = "nvarchar(300)")]
        [Required]
        public string RecipeTitle { get; set; } = string.Empty;

        [Column("content", TypeName = "nvarchar(2000)")]
        [Required]
        public string? Content { get; set; }

        [Column("category_id", TypeName = "char(36)")]
        [ForeignKey("CategoryId")]
        [Required]
        public Guid CategoryId { get; set; }

        public Category? Category { get; set; }

        public ICollection<IngredientRecipe>? IngredientRecipes { get; set; }

    }
}