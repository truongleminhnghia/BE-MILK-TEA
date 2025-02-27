using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper, IAccountRepository accountRepository)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _accountRepository = accountRepository;
        }

        public async Task<Employee> CreateEmployee(CreateStaffRequest createStaffRequest)
        {
            try
            {
                var customer = _mapper.Map<Employee>(createStaffRequest);

                var result = await _employeeRepository.Create(customer);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }

        public async Task<Employee> UpdateEmployee(Guid id, UpdateStaffRequest updateStaffRequest)
        {
            try
            {
                var employee = await _employeeRepository.GetById(id);
                if (employee == null)
                {
                    throw new Exception("Tài khoản không tồn tại");
                }
                employee.UpdateAt = DateTime.Now;
                employee.RefCode = updateStaffRequest.RefCode;
                var result = await _employeeRepository.UpdateEmployee(employee);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<Employee>> GetAllEmployee()
        {
            try
            {
                var result = await _employeeRepository.GetAllEmployee();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }

        public async Task<Employee> GetById(Guid _id)
        {
            try
            {
                var result = await _employeeRepository.GetById(_id);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error: " + ex.Message);
                return null;
            }
        }
    }
}
