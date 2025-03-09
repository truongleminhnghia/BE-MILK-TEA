using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Data_Access_Layer.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> Create(Customer _customer);

        Task<Customer> UpdateCustomer(Customer id);
        Task<Customer> GetByAccountId(Guid _id);
        Task<IEnumerable<Customer>> GetAllCustomer();
    }
}