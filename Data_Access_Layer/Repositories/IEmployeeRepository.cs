using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> Create(Employee _employee);
        Task<Employee> GetById(Guid _id);
        Task<Employee> UpdateEmployee(Employee _employee);

        Task<IEnumerable<Employee>> GetAllEmployee();
    }
}
