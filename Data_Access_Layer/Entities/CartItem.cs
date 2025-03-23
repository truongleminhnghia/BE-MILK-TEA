using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Data_Access_Layer.Entities
{
        [Table("cart_item")]
        public class CartItem : BaseEntity
        {
                [Key]
                [Column("cart_item_id")]
                [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
                public Guid Id { get; set; }

                [Column("quantity")]
                [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
                [Required]
                public int Quantity { get; set; }
                
                [Column("product_type")]
                [EnumDataType(typeof(ProductType))]
                [Required]
                public ProductType ProductType { get; set; }

                [Column("cart_id")]
                [Required]
                [ForeignKey("CartId")]
                public Guid CartId { get; set; }

                [ForeignKey("IngredientId")]
                [Required]
                public Guid IngredientId { get; set; }

                [Column("price")]
                public double Price { get; set; }

                [Column("total_price")]
                public double? TotalPrice { get; set; }

                [Column("isCart")]
                [Required]
                public bool IsCart { get; set; }

                public Cart? Cart { get; set; }
                public Ingredient? Ingredient { get; set; }
        }
}