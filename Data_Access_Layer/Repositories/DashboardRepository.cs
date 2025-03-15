using Data_Access_Layer.Data;
using Data_Access_Layer.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<double> GetTotalEarningsAsync()
        {
            return await _context.Orders
                .Where(p => p.OrderStatus == OrderStatus.CONFIRM)
                .SumAsync(p => p.TotalPrice);
        }

        public async Task<int> GetTotalOrdersAsync()
        {
            return await _context.Orders
                .Where(p => p.OrderStatus == OrderStatus.CONFIRM)
                .CountAsync();
        }

        public async Task<int> GetTotalAccountsAsync()
        {
            return await _context.Accounts
                .Where (p => p.RoleName == RoleName.ROLE_CUSTOMER)
                .CountAsync();
        }

        public async Task<int> GetTotalProductsAsync()
        {
            return await _context.Ingredients
                .CountAsync();
        }
    }
}
