using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services.DashboardService
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardResponse> GetDashboardDataAsync()
        {
            return new DashboardResponse
            {
                TotalEarnings = await _dashboardRepository.GetTotalEarningsAsync(),
                TotalOrders = await _dashboardRepository.GetTotalOrdersAsync(),
                TotalRegisteredAccounts = await _dashboardRepository.GetTotalAccountsAsync(),
                TotalProducts = await _dashboardRepository.GetTotalProductsAsync()
            };
        }
    }
}
