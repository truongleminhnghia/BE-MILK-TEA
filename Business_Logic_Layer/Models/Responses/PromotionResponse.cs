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
        public Guid? Id { get; set; }
        public string? PromotionCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public PromotionType PromotionType { get; set; }
        public PromotionDetailResponse PromotionDetails { get; set; } = new PromotionDetailResponse();
    }

    public class ActivePromotionResponse
    {
        public Guid? Id { get; set; }
        public string? PromotionCode { get; set; }
    }
}
