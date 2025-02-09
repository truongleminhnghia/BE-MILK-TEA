using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    public class Image
    {
        [Key]
        [Column("image_id")]
        public Guid ImageId { get; set; }

        [Column("image_url")]
        public string ImageUrl { get; set; }

        [Column("ingredient_id")]
        public Guid IngredientId { get; set; }

        // Setup relationship
        public virtual Ingredient Ingredient { get; set; }

        // Constructor
        public Image(string imageUrl, Guid ingredientId)
        {
            ImageId = Guid.NewGuid();
            ImageUrl = imageUrl;
            IngredientId = ingredientId;
        }
    }
}
