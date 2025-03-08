using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Data_Access_Layer.Entities
{
    [Table("ingredient_quantity")]
    public class IngredientQuantity : BaseEntity
    {
        [Key]
        [Column("ingredient_quantity_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("ingredient_id")]
        [Required]
        [ForeignKey("IngredientId")]
        public Guid IngredientId { get; set; }

        [Column("quantity")]
        [Range(1, int.MinValue, ErrorMessage = "Quantity must be greater than 0")]
        [Required]
        public int Quantity { get; set; }

        [Column("product_type", TypeName = "nvarchar(200)")]
        [Required]
        [EnumDataType(typeof(ProductType))]
        public ProductType ProductType { get; set; }

        public Ingredient Ingredients { get; set; }


    }
}
