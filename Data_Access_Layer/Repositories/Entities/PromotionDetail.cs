using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories.Entities
{
    [Table("promotion_detail")]
    public class PromotionDetail
    {
        [Key]
        [Column("promotion_detail_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Column("promotion_name", TypeName = "nvarchar(300)")]
        public string? PromotionName { get; set; } 

        [Column("description", TypeName = "nvarchar(300)")]
        public string? Description { get; set; } 

        [Column("discount_value")]
        [Required]
        public float DiscountValue { get; set; } 

        [Column("mini_value")]
        public double MiniValue { get; set; }

        [Column("max_value")]
        public double MaxValue { get; set; }

        [Column("promtion_id")]
        [ForeignKey("PromotionId")]
        public Guid PromotionId { get; set; }

        public Promotion? Promotion { get; set; }
    }
}