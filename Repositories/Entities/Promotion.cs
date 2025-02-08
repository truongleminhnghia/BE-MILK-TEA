using Repositories.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    public class Promotion
    {
        [Key]
        [Column("promotion_id")]
        public Guid Promotion_Id { get; set; } = Guid.NewGuid();

        [Column("promotion_code", TypeName = "varchar(300)")]
        public string Promotion_Code { get; set; }

        [Column("start_date")]
        public DateTime Start_Date { get; set; }

        [Column("end_date")]
        public DateTime End_Date { get; set; }

        [Column("promotion_type")]
        public PromotionTypeEnum Promotion_Type { get; set; }

        [Column("is_active")]
        public bool Is_Active { get; set; } = false;

        //     setup relationship
        public virtual ICollection<Ingredient_Promotion> Ingredients_Promotions { get; set; }
        public virtual ICollection<Order_Promotion> Orders_Promotions { get; set; }
        public virtual Promotion_Detail Promotion_Detail { get; set; }
    }
}
