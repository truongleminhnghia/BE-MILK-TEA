﻿using Repositories.Enums;
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
        public Guid PromotionId { get; set; }

        [Column("promotion_code", TypeName = "varchar(300)")]
        public string PromotionCode { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("promotion_type")]
        public PromotionTypeEnum PromotionType { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = false;

        // Setup relationships
        public virtual ICollection<IngredientPromotion> IngredientPromotions { get; set; }
        public virtual ICollection<OrderPromotion> OrderPromotions { get; set; }
        public virtual PromotionDetail PromotionDetail { get; set; }

        // Constructor
        public Promotion(string promotionCode, DateTime startDate, DateTime endDate, PromotionTypeEnum promotionType)
        {
            PromotionId = Guid.NewGuid();
            PromotionCode = promotionCode;
            StartDate = startDate;
            EndDate = endDate;
            PromotionType = promotionType;
        }
    }
}
