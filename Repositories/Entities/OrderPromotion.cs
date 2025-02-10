using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Entities
{
    [Table("order_promotion")]
    public class OrderPromotion
    {
        public Guid OrderId { get; set; }
        public Guid PromotionId { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Promotion Promotion { get; set; }
    }
}