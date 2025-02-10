using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    [Table("image")]
    public class Image
    {
        [Key]
        [Column("image_id")]
        public Guid ImageId { get; set; } = Guid.NewGuid();

        [Column("image_url")]
        public string ImageUrl { get; set; }

        [Column("ingredient_id")]
        public Guid IngredientId { get; set; }

        // Setup relationship
        public virtual Ingredient Ingredient { get; set; }

        // Constructor
        public Image()
        {
        }
        public Image(string imageUrl, Guid ingredientId)
        {
            ImageUrl = imageUrl;
            IngredientId = ingredientId;
        }
    }
}
