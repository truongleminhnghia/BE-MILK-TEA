
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;


namespace Business_Logic_Layer.Models.Requests
{
    public class PromotionRequest
    {
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PromotionType PromotionType { get; set; }
        //public List<Guid> IngredientId { get; set; } = new List<Guid>();
        public PromotionDetailRequest promotionDetail { get; set; }
    }
}
