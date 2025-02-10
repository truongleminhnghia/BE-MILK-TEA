using Repositories.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Repositories.Entities
{
    [Table("ingredient")]
    public partial class Ingredient : BaseEntity
    {
        [Key]
        [Column("ingredient_id")]
        public Guid IngredientId { get; set; } = Guid.NewGuid();

        [Column("ingredient_code", TypeName = "varchar(300)")]
        public string IngredientCode { get; set; }

        [Column("supplier", TypeName = "nvarchar(300)")]
        public string Supplier { get; set; }

        [Column("description", TypeName = "nvarchar(500)")]
        public string Description { get; set; }

        [Column("food_safety_certification")]
        public string FoodSafetyCertification { get; set; }

        [Column("expired_date")]
        public DateTime ExpiredDate { get; set; }

        [Column("image_id")]
        public Guid ImageId { get; set; }

        [Column("ingredient_status")]
        public IngredientStatusEnum IngredientStatus { get; set; }

        //gram per package
        [Column("unit")]
        public float Unit { get; set; }

        //final price after promotion = price - price * promotion or price - promotion
        [Column("price_promotion")]
        public int PricePromotion { get; set; }

        [Column("category_id")]
        public Guid CategoryId { get; set; }

        //      setup relationship
        public virtual Category Category { get; set; }
        public virtual ICollection<Image> Images { get; set; } = new List<Image>();
        public virtual ICollection<IngredientPromotion>? IngredientsPromotions { get; set; } = new List<IngredientPromotion>();
        public virtual ICollection<IngredientRecipe> IngredientRecipes { get; set; }
        public virtual ICollection<IngredientProduct> Products { get; set; } = new List<IngredientProduct>();

        //      constructor
        public Ingredient()
        {
        }
        public Ingredient(string ingredientCode, string supplier, string description, string foodSafetyCertification, DateTime expiredDate, Guid imageId, IngredientStatusEnum ingredientStatus, float unit, int pricePromotion, Guid categoryId)
        {
            //IngredientCode = ingredientCode ?? throw new ArgumentNullException(nameof(ingredientCode));
            //Supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
            //Description = description;
            //FoodSafetyCertification = foodSafetyCertification ?? throw new ArgumentNullException(nameof(foodSafetyCertification));
            IngredientCode = ingredientCode;
            Supplier = supplier;
            Description = description;
            FoodSafetyCertification = foodSafetyCertification;
            ExpiredDate = expiredDate;
            ImageId = imageId;
            IngredientStatus = ingredientStatus;
            Unit = unit;
            PricePromotion = pricePromotion;
            CategoryId = categoryId;

            // Initialize collections
            Images = new List<Image>();
            IngredientsPromotions = new List<IngredientPromotion>();
            IngredientRecipes = new List<IngredientRecipe>();
            Products = new List<IngredientProduct>();
        }
    }
}
