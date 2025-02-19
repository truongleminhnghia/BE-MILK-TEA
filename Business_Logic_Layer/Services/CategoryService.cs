using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic_Layer.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _context;

        public CategoryService(ICategoryRepository context)
        {
            _context = context;
        }

        public Task<bool> CategoryExistsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Category> CreateAsync(Category category)
        {
            return await _context.CreateAsync(category);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Category>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Category> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Category> UpdateAsync(Category category)
        {
            throw new NotImplementedException();
        }
    }
}
