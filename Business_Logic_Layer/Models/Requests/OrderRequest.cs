﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Enum;
using Business_Logic_Layer.DTO;

namespace Business_Logic_Layer.Models.Requests
{
    public class OrderRequest
    {
        public string OrderCode { get; set; } = string.Empty;
        public OrderStatus OrderStatus { get; set; }

        public string FullNameShipping { get; set; } = string.Empty;
        public string PhoneShipping { get; set; } = string.Empty;
        public string? EmailShipping { get; set; }
        public string? NoteShipping { get; set; }
        public string AddressShipping { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public string RefCode { get; set; } = string.Empty;
        public Guid AccountId { get; set; }
        public List<CartDetailDTO> cartDetailList { get; set; } =new List<CartDetailDTO>() ;
      
    }
}
