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
        [Column ("image_id")]
        public Guid Image_Id { get; set; } = Guid.NewGuid();

        [Column("image_url")]
        public string Image_Url { get; set; }

        [Column("ingredient_id")]
        public Guid Ingredient_Id { get; set; }

        //      setup relationship
        public virtual Ingredient Ingredient { get; set; }
    }
}
