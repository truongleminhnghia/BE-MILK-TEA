﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Requests
{
    public class RemoveCartItemRequest
    {
        public Guid AccountId { get; set; }
        public Guid IngredientProductId { get; set; }
    }
}
