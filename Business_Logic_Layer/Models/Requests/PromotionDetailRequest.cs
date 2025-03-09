using System;

namespace Business_Logic_Layer.Models.Requests
{
    public class PromotionDetaislRequest
    {
        public Guid PromotionId { get; set; }
        public string? PromotionName { get; set; } // ✅ Thêm vào
        public string? Description { get; set; } // ✅ Thêm vào
        public float DiscountValue { get; set; } // ✅ Đổi từ DiscountPercent
        public double MiniValue { get; set; } // ✅ Thêm vào
        public double MaxValue { get; set; } // ✅ Thêm vào
    }
}
