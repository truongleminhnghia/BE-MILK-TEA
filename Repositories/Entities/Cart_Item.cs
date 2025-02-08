using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Entities
{
    public class Cart_Item
    {
        [Key]
        [Column("cart_item_id")]
        public Guid Cart_Item_Id { get; set; } = Guid.NewGuid();

        [Column("cart_id")]
        public Guid Cart_Id { get; set; }

        [Column("item_id")]
        public Guid Product_Id { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        //      setup relationship
        public virtual Cart Cart { get; set; }
        public virtual Ingredient_Product Product { get; set; }
    }
}