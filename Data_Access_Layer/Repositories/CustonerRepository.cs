using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public class CustonerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustonerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> Create (Customer _customer)
        {
            _context.Customers.AddAsync(_customer);
            await _context.SaveChangesAsync();
            return _customer;
        }
    }
}
