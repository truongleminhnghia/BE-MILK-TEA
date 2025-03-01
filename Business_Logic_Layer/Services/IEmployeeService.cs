using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;

namespace Business_Logic_Layer.Services
{
    public interface IEmployeeService
    {
        Task<Employee> CreateEmployee(CreateStaffRequest createStaffRequest);
        Task<Employee> UpdateEmployee(Guid id, UpdateStaffRequest updateStaffRequest);
        Task<Employee> GetById(Guid _id);
        Task<IEnumerable<Employee>> GetAllEmployee();

    }
}
