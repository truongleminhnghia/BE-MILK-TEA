﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.Models.Requests
{
    public class OrderDetailRequest
    {
        public Guid IngredientProductId { get; set; }
        public int Quantity { get; set; }
    }
}
