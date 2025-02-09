namespace Repositories.Entities
{
    public class OrderPromotion
    {
        public Guid OrderId { get; set; }
        public Guid PromotionId { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Promotion Promotion { get; set; }
    }
}