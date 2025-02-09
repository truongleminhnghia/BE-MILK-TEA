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
        public Guid CategoryId { get; set; }

        [Column("category_name", TypeName = "nvarchar(300)")]
        public string CategoryName { get; set; }

        [Column("category_status")]
        public CategoryStatusEnum CategoryStatus { get; set; }

        [Column("category_type")]
        public string CategoryType { get; set; }

        // Setup relationship
        public virtual ICollection<Ingredient>? Ingredients { get; set; } = new List<Ingredient>();
        public virtual ICollection<Recipe>? Recipes { get; set; } = new List<Recipe>();

        // Constructor
        public Category(string categoryName, CategoryStatusEnum categoryStatus, string categoryType)
        {
            CategoryId = Guid.NewGuid();
            CategoryName = categoryName;
            CategoryStatus = categoryStatus;
            CategoryType = categoryType;
        }
    }
}
