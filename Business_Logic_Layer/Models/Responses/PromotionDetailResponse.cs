using System;

namespace Business_Logic_Layer.Models.Responses
{
    public class PromotionDetailResponse
    {
        public Guid Id { get; set; }
        public Guid PromotionId { get; set; }
        public string? PromotionName { get; set; } // ✅ Thêm vào
        public string? Description { get; set; } // ✅ Thêm vào
        public float DiscountPercent { get; set; } // ✅ Đổi từ DiscountValue
        public double MiniValue { get; set; } // ✅ Thêm vào
        public double MaxValue { get; set; } // ✅ Thêm vào
    }

}
