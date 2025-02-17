using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories.Entities
{
    [Table("order_promotion")]
    public class OrderPromotion
    {
        [Key]
        [Column("order_promotion_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("order_id")]
        [ForeignKey("OrderId")]
        public Guid OrderId { get; set; }

        [Column("promotion_id")]
        [ForeignKey("PromotionId")]
        public Guid PromotionId { get; set; }

        public Promotion? Promotion { get; set; }
        public Order? Order { get; set; }
    }
}