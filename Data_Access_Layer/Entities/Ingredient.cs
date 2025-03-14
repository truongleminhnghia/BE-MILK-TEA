using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Data_Access_Layer.Entities
{
    [Table("ingredient")]
    public class Ingredient : BaseEntity
    {
        [Key]
        [Column("ingredient_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        // render auto with PCxxxxx
        [Column("ingredient_code", TypeName = "nvarchar(100)")]
        [Required(ErrorMessage = "Ingredient Code cannot be null")]
        public string IngredientCode { get; set; } = string.Empty;

        [Column("supplier", TypeName = "nvarchar(300)")]
        [Required]
        public string Supplier { get; set; } = string.Empty;

        [Column("ingredient_name", TypeName = "nvarchar(300)")]
        [Required]
        public string IngredientName { get; set; } = string.Empty;

        [Column("description", TypeName = "nvarchar(500)")]
        public string Description { get; set; } = string.Empty;

        [Column("food_safety_certification", TypeName = "nvarchar(300)")]
        [Required]
        public string FoodSafetyCertification { get; set; } = string.Empty;

        [Column("expired_date", TypeName = "datetime")]
        [Required]
        public DateTime ExpiredDate { get; set; }

        [Column("ingredient_status")]
        [Required]
        public IngredientStatus IngredientStatus { get; set; }

        [Column("weight_per_bag")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public float WeightPerBag { get; set; } // trong lượng trong mỗi bịch => một bịch 500g 0.5kg......

        [Column("quantity_per_carton")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int QuantityPerCarton { get; set; } // số lượng trong mỗi thùng ==> thùng 12 bịch

        [Column("ingredient_type")]
        [EnumDataType(typeof(IngredientType))]
        [Required]
        public IngredientType IngredientType  { get; set; } // Thêm Enum mới

        [Column("unit", TypeName = "nvarchar(50)")]
        [EnumDataType(typeof(UnitOfIngredientEnum))]
        public UnitOfIngredientEnum Unit { get; set; } // đơn vị là string, viết kilogram hoặc gram

        [Column("price_origin")]
        [Required]
        public double PriceOrigin { get; set; } // giá ban đầu, không khuyến mãi

        [Column("price_promotion")]
        [Required]
        public double PricePromotion { get; set; } // giá khuyến mãi

        [Column("category_id")]
        [Required]
        [ForeignKey("CategoryId")]
        public Guid CategoryId { get; set; }

        [Column("is_sale")]
        [Required]
        public bool IsSale { get; set; }

        [Column("rate")]
        public float Rate { get; set; } // tính TB 4 + 5 /2
         // bổ sung bình luận

        public ICollection<Image>? Images { get; set; }
        public ICollection<IngredientReview>? IngredientReviews { get; set; }
        public ICollection<IngredientProduct>? IngredientProducts { get; set; }
        public ICollection<IngredientPromotion>? IngredientPromotions { get; set; }
        public ICollection<IngredientRecipe>? IngredientRecipes { get; set; }
        public Category? Category { get; set; }
        public ICollection<IngredientQuantity>? IngredientQuantities { get; set; }
    }
    
}

// isStaff = true ==> crud
// customer: xemmmm
