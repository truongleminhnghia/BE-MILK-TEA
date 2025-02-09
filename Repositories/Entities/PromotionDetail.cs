using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Entities
{

    public class PromotionDetail
    {
        [Key]
        [Column("promotion_detail_id")]
        public Guid PromotionDetailId { get; set; }

        [Column("promotion_name", TypeName = "nvarchar(300)")]
        public string PromotionName { get; set; }

        [Column("promotion_description", TypeName = "nvarchar(300)")]
        public string PromotionDescription { get; set; }

        // percentage discount
        [Column("promotion_discount")]
        public int PromotionDiscount { get; set; }

        // minimum order's total amount to apply promotion
        [Column("minimum_order_value")]
        public int MinimumOrderValue { get; set; }

        // maximum order's total amount to apply promotion. If over this value, the discount will be applied with the maximum discount value
        [Column("maximum_discount")]
        public int MaximumDiscount { get; set; }

        [Column("promotion_id")]
        public Guid PromotionId { get; set; }

        // setup relationship
        public virtual Promotion Promotion { get; set; }

        // constructor
        public PromotionDetail(string promotionName, string promotionDescription, int promotionDiscount, int minimumOrderValue, int maximumDiscount, Guid promotionId)
        {
            PromotionDetailId = Guid.NewGuid();
            PromotionName = promotionName;
            PromotionDescription = promotionDescription;
            PromotionDiscount = promotionDiscount;
            MinimumOrderValue = minimumOrderValue;
            MaximumDiscount = maximumDiscount;
            PromotionId = promotionId;
        }
    }
}
