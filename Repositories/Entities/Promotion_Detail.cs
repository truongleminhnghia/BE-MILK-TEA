using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    public class Promotion_Detail
    {
        [Key]
        [Column("promotion_detail_id")]
        public Guid Promotion_Detail_Id { get; set; } = Guid.NewGuid();

        [Column("promotion_name", TypeName = "nvarchar(300)")]
        public string Promotion_Name { get; set; }

        [Column("promotion_description", TypeName = "nvarchar(300)")]
        public string Promotion_Description { get; set; }

        //percentage discount
        [Column("promotion_discount")]
        public int Promotion_Discount { get; set; }

        //minumum order's total amount to apply promotion
        [Column("Minimum_Order_Value")]
        public int Minimum_Order_Value { get; set; }

        //maximum order's total amount to apply promotion. If over this value, the discount will be applied with the maximum discount value
        [Column("Maximum_Discount")]
        public int Maximum_Discount { get; set; }

        [Column("promotion_id")]
        public Guid Promotion_Id { get; set; }

        //      setup relationship
        public virtual Promotion Promotion { get; set; }
    }
}
