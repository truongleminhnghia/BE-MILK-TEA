using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Access_Layer.Entities
{
    [Table("image")]
    public class Image
    {
        [Key]
        [Column("image_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } 

        [Column("image_url", TypeName = "nvarchar(1000)")] // URL
        [Required]
        public string ImageUrl { get; set; }

        [Column("ingredient_id")]
        [ForeignKey("IngredientId")]
        [Required]
        public Guid IngredientId { get; set; }

        // relationship
        // N-1 Ingredient
        public Ingredient? Ingredient { get; set; }
    }
}