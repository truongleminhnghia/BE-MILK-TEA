using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Repositories.Entities
{
    public class Order_Detail
    {
        [Key]
        [Column("order_detail_id")]
        public Guid Order_Detail_Id { get; set; }

        [Column("order_id")]
        public Guid Order_Id { get; set; }

        [Column("ingredient_product_id")]
        public Guid Product_Id { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("total_amount")]
        public double Total_Amount { get; set; }

        //      setup relationship
        public virtual Order Order { get; set; }
        public virtual Ingredient_Product Ingredient_Product { get; set; }
    }
}