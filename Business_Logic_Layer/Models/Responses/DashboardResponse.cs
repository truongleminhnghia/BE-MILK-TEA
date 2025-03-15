using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models.Responses
{
    public class DashboardResponse
    {
        public double TotalEarnings { get; set; }
        public int TotalOrders { get; set; }
        public int TotalRegisteredAccounts { get; set; }
        public int TotalProducts { get; set; }
    }
}
