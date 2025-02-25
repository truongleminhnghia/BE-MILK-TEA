using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> Create(Customer _customer);
    }
}