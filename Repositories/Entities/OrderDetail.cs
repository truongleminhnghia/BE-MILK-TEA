using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Entities
{
    public class OrderDetail
    {
        [Key]
        [Column("order_detail_id")]
        public Guid OrderDetailId { get; set; }

        [Column("order_id")]
        public Guid OrderId { get; set; }

        [Column("ingredient_product_id")]
        public Guid IngredientProductId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("total_amount")]
        public double TotalAmount { get; set; }

        // Setup relationship
        public virtual Order Order { get; set; }
        public virtual IngredientProduct IngredientProduct { get; set; }

        // constructor
        public OrderDetail(Guid orderId, Guid productId, int quantity, double totalAmount)
        {
            OrderDetailId = Guid.NewGuid();
            OrderId = orderId;
            IngredientProductId = productId;
            Quantity = quantity;
            TotalAmount = totalAmount;
        }
    }
}