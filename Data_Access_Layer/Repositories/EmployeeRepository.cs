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
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Employee> Create(Employee _employee)
        {
            _context.Employees.AddAsync(_employee);
            await _context.SaveChangesAsync();
            return _employee;
        }

        public async Task<Employee> GetById(Guid _id)
        {
            return await _context.Employees.FirstOrDefaultAsync(s => s.AccountId == _id) ;
        }

        public async Task<Employee> UpdateEmployee(Employee _employee)
        {
            _context.Employees.Update(_employee);
            await _context.SaveChangesAsync();
            return _employee;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployee()
        {
            return await Task.FromResult(_context.Employees.ToList());
        }

        public async Task<bool> CheckRefCode(string refCode)
        {
            return await _context.Employees.AnyAsync(e => e.RefCode == refCode);
        }
    }
}
