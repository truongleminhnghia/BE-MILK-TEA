using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> Create (Customer _customer)
        {
            _context.Customers.AddAsync(_customer);
            await _context.SaveChangesAsync();
            return _customer;
        }

        public async Task<Customer> GetByAccountId(Guid _id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.AccountId == _id);
        }

        public async Task<Customer> UpdateCustomer(Customer _customer)
        {
            _context.Customers.Update(_customer);
            await _context.SaveChangesAsync();
            return _customer;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomer()
        {
            return await _context.Customers.ToListAsync();
        }
    }
}
