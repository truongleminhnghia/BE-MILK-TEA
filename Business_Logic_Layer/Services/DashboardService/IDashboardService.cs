using Business_Logic_Layer.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services.DashboardService
{
    public interface IDashboardService
    {
        Task<DashboardResponse> GetDashboardDataAsync();
    }
}
