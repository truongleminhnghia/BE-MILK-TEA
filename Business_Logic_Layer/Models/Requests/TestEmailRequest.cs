﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Requests
{
    public class TestEmailRequest
    {
        public string Email { get; set; }
        public int AmountPaid { get; set; }
    }
}
