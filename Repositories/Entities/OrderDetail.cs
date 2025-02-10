﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Entities
{
    [Table("order_detail")]
    public class OrderDetail
    {
        [Key]
        [Column("order_detail_id")]
        public Guid OrderDetailId { get; set; } = Guid.NewGuid();

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
        public OrderDetail()
        {
        }
        public OrderDetail(Guid orderId, Guid productId, int quantity, double totalAmount)
        {
            OrderId = orderId;
            IngredientProductId = productId;
            Quantity = quantity;
            TotalAmount = totalAmount;
        }
    }
}