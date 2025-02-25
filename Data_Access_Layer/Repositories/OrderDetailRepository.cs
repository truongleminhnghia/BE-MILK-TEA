using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer.Repositories
{
    public class OrderDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<OrderDetail>> GetOrderDetailsAsync()
        {
            try
            {
                var response = await _context.OrderDetails.ToListAsync();
                return response;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //public async Task<OrderDetail?> GetByIdAsync(Guid id)
        //{
        //    return await _context.OrdersDetails.FirstOrDefaultAsync(o => o.Id == id) ?? null;
        //}
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
    }
}
