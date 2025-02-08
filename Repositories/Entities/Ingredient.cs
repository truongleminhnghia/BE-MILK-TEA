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
    public partial class Ingredient : BaseEntity
    {
        [Key]
        [Column ("ingredient_id")]
        public Guid Ingredient_Id { get; set; } = Guid.NewGuid();

        [Column("ingredient_code", TypeName = "varchar(300)")]
        public string Ingredient_Code { get; set; }

        [Column("supplier", TypeName = "nvarchar(300)")]
        public string Supplier { get; set; }

        [Column("description", TypeName = "nvarchar(500)")]
        public string Description { get; set; }

        [Column ("food_safety_certification")]
        public string Food_Safety_Certification { get; set; }

        [Column ("expired_date")]
        public DateTime Expired_Date { get; set; }

        [Column ("image_id")]
        public Guid Image_ID { get; set; }

        [Column ("ingredient_status")]
        public IngredientStatusEnum Ingredient_Status { get; set; }

        //gram per package
        [Column ("unit")]
        public float Unit { get; set; }

        //final price after promotion = price - price * promotion or price - promotion
        [Column("price_promotion")]
        public int Price_Promotion { get; set; }        

        [Column("category_id")]
        public Guid Category_Id { get; set; }

        //      setup relationship
        public virtual Category Category { get; set; }
        public virtual ICollection<Image> Images { get; set; } = new List<Image>();
        public virtual ICollection<Ingredient_Promotion>? Ingredients_Promotions { get; set; } = new List<Ingredient_Promotion>();
        public virtual ICollection<Ingredient_Recipe> Ingredient_Recipes { get; set; }
        public virtual ICollection<Ingredient_Product> Products { get; set; } = new List<Ingredient_Product>();
    }
}
