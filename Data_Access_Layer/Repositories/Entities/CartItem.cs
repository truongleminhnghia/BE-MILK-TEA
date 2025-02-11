using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories.Entities
{
    [Table("cart_item")]
    public class CartItem : BaseEntity
    {
        [Key]
        [Column("cart_item_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("quantity")]
        [Required]
        public int Quantity { get; set; }

        [Column("cart_id")]
        [Required]
        [ForeignKey("cart_id")]
        public Guid CartId { get; set; }

        [Column("ingredient_product_id")]
        [Required]
        [ForeignKey("ingredient_product_id")]
        public Guid IngredientProductId { get; set; }
        public Cart? Cart { get; set; }
        public IngredientProduct? IngredientProduct { get; set; }

        // relationship
        // N-1 Cart
        // 1-1 Product

    }
}