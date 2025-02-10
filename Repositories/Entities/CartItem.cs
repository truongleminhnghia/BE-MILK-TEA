using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Entities
{
    [Table("cart_item")]
    public class CartItem
    {
        [Key]
        [Column("cart_item_id")]
        public Guid CartItemId { get; set; } = Guid.NewGuid();

        [Column("cart_id")]
        public Guid CartId { get; set; }

        [Column("item_id")]
        public Guid ProductId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        // Setup relationship
        public virtual Cart Cart { get; set; }
        public virtual IngredientProduct Product { get; set; }

        // Constructor
        public CartItem()
        {
        }
        public CartItem(Guid cartId, Guid productId, int quantity)
        {
            CartId = cartId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}