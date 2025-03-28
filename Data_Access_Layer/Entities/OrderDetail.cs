using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Access_Layer.Entities
{
    [Table("order_detail")]
    public class OrderDetail
    {
        [Key]
        [Column("order_detail_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("quantity")]
        [Required]
        public int Quantity { get; set; }

        [Column("price")]
        [Required]
        public double Price { get; set; }

        [Column("order_id")]
        [ForeignKey("OrderId")]
        [Required]
        public Guid OrderId { get; set; }

        [Column("cart_item_id")]
        [ForeignKey("CartItemId")]
        [Required]
        public Guid CartItemId { get; set; }

        public virtual Order? Orders { get; set; }

        public CartItem? CartItems { get; set; }

    }
}