using Repositories.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    public class Category
    {
        [Key]
        [Column("category_id")]
        public Guid Category_Id { get; set; } = Guid.NewGuid();

        [Column("category_name", TypeName = "nvarchar(300)")]
        public string Category_Name { get; set; }

        [Column("category_status")]
        public CategoryStatusEnum Category_Status { get; set; }

        [Column("category_type")]
        public string Category_Type { get; set; }

        //      setup relationship
        public virtual ICollection<Ingredient>? Ingredients { get; set; } = new List<Ingredient>();
        public virtual ICollection<Recipe>? Recipes { get; set; } = new List<Recipe>();
    }
}
