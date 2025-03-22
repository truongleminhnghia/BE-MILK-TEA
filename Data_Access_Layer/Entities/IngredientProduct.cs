using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Data_Access_Layer.Entities
{
    [Table("ingredient_product")]
    public class IngredientProduct
    {
        [Key]
        [Column("ingredient_product_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("ingredient_id")]
        [Required]
        [ForeignKey("IngredientId")]
        public Guid IngredientId { get; set; }

        [Column("total_price")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public double TotalPrice { get; set; }

        [Column("quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Column("product_type")]
        [EnumDataType(typeof(ProductType))]
        [Required]
        public ProductType ProductType { get; set; }

        
        public Ingredient? Ingredient { get; set; }
    }    
}