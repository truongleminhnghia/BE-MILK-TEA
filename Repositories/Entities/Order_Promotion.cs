namespace Repositories.Entities
{
    public class Order_Promotion
    {
        public Guid Order_Id { get; set; }
        public Guid Promotion_Id { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Promotion Promotion { get; set; }
    }
}