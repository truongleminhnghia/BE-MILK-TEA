using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Responses
{
    public class PromotionResponse
    {
        public String? PromotionCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public PromotionType PromotionType { get; set; }
        public List<PromotionDetailResponse> PromotionDetails { get; set; } = new List<PromotionDetailResponse>();

        public void ConvertToPromotionDetailResponse(List<PromotionDetail> details, IMapper mapper)
        {
            this.PromotionDetails = details.Select(d => mapper.Map<PromotionDetailResponse>(d)).ToList();
        }
    }
}
