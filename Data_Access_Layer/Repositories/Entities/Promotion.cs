using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Data_Access_Layer.Repositories.Entities
{
    [Table("promotion")]
    public class Promotion : BaseEntity
    {
        [Key]
        [Column("promotion_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } 

        [Column("promotion_code")]
        [Required]
        public string PromotionCode { get; set; } = string.Empty;

        [Column("promotion_detail_id")]
        [Required]
        public Guid PromotionDetailId { get; set; }

        [Column("is_active")]
        [Required]
        public bool IsActive { get; set; }

        [Column("start_date", TypeName = "datetime")]
        [Required]
        public DateTime StartDate { get; set; }

        [Column("end_date", TypeName = "datetime")]
        [Required]
        public DateTime EndDate { get; set; }

        [Column("promotion_type")]
        [Required]
        public PromotionType PromotionType { get; set; }

        public PromotionDetail? PromotionDetail { get; set; }
        
        public ICollection<OrderPromotion>? OrderPromotions { get; set; }

    }
}