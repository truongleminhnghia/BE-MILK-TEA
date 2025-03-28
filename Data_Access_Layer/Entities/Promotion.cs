using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Data_Access_Layer.Entities
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

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("start_date", TypeName = "datetime")]
        [Required]
        public DateTime StartDate { get; set; }

        [Column("end_date", TypeName = "datetime")]
        [Required]
        public DateTime EndDate { get; set; }

        [Column("promotion_type")]
        [EnumDataType(typeof(PromotionType))]
        public PromotionType PromotionType { get; set; }


        //relationship
        public PromotionDetail? PromotionDetail { get; set; }
     
        public ICollection<OrderPromotion>? OrderPromotions { get; set; }

        public ICollection<IngredientPromotion>? IngredientPromotions { get; set; }

    }
}