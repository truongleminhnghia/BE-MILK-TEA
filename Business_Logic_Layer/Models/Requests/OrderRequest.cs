﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;

namespace Business_Logic_Layer.Models.Requests
{
    public class OrderRequest
    {
        public string FullNameShipping { get; set; } = string.Empty;
        public string PhoneShipping { get; set; } = string.Empty;
        public string? EmailShipping { get; set; }
        public string? NoteShipping { get; set; }
        public string AddressShipping { get; set; } = string.Empty;
   
        //Người dùng nhập promotion code ở đây
        public string PromotionCode { get; set; } = string.Empty;
        public Guid AccountId { get; set; }
        public List<OrderDetailRequest> orderDetailList { get; set; } =new List<OrderDetailRequest>() ;
    

    }
}
