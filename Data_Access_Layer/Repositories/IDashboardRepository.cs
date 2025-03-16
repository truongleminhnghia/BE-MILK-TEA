using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public interface IDashboardRepository
    {
        Task<double> GetTotalEarningsAsync();
        Task<int> GetTotalOrdersAsync();
        Task<int> GetTotalAccountsAsync();
        Task<int> GetTotalProductsAsync();
    }
}
