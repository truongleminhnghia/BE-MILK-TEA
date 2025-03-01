using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;

namespace Data_Access_Layer.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<OrderDetail>> GetAllOrdersDetailAsync(Guid orderId, string? search, string? sortBy, bool isDescending, int page, int pageSize)
        {
            try
            {
                var query = _context.OrderDetails.AsQueryable();

                query = query.Where(od => od.OrderId == orderId);

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(od => od.IngredientProductId.ToString().Contains(search));
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    query = isDescending
                        ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                        : query.OrderBy(e => EF.Property<object>(e, sortBy));
                }

                query = query.Skip((Math.Max(1, page) - 1) * pageSize).Take(pageSize);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể lọc được order detail: {ex.Message}", ex);
            }
        }

        public async Task<OrderDetail> CreateAsync(OrderDetail orderDetail)
        {
            orderDetail.Id = Guid.NewGuid();

            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
            return orderDetail;
        }
        public async Task<bool> DeleteByIdAsync(Guid id)
        {
            var existingOrderDetail = await _context.OrderDetails.FindAsync(id);
            if (existingOrderDetail == null)
            {
                return false;
            }
            _context.OrderDetails.Remove(existingOrderDetail);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<OrderDetail?> GetByIdAsync(Guid id)
        {
            return await _context.OrderDetails.FirstOrDefaultAsync(c => c.Id == id);
        }
       public async Task<OrderDetail?> UpdateAsync(Guid id, OrderDetail orderDetail)
        {
            var existingOrderDetail = await _context.OrderDetails.FindAsync(id);
            if (existingOrderDetail == null)
            {
                return null;
            }
            var ingredient = await _context.IngredientProducts.FirstOrDefaultAsync(i => i.Id == orderDetail.IngredientProductId);
            if (ingredient == null)
            {
                throw new Exception("Ingredient not found.");
            }
            existingOrderDetail.Quantity = orderDetail.Quantity;
            existingOrderDetail.Price = orderDetail.Quantity * ingredient.TotalPrice;
            await _context.SaveChangesAsync();
            return existingOrderDetail;
        }
    }
}
