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
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            try { 
                var response = await _context.Orders.ToListAsync();
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders.Include(o => o.OrderDetails)
                                        .Include(o => o.Payments)
                                        .Include(o => o.OrderPromotions)
                                        .FirstOrDefaultAsync(o => o.Id == id) ?? null;
        }
        public async Task<Order> CreateAsync(Order order)
        {
            order.Id = Guid.NewGuid();
            order.OrderDate = DateTime.Now;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateAsync(Guid id, Order order)
        {
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                return null;
            }
            existingOrder.OrderCode = order.OrderCode;
            existingOrder.OrderDate = order.OrderDate;
            existingOrder.FullNameShipping = order.FullNameShipping;
            existingOrder.PhoneShipping = order.PhoneShipping;
            existingOrder.EmailShipping = order.EmailShipping;
            existingOrder.NoteShipping = order.NoteShipping;
            existingOrder.AddressShipping = order.AddressShipping;
            existingOrder.OrderStatus = order.OrderStatus;
            existingOrder.Quantity = order.Quantity;
            existingOrder.PriceAfterPromotion = order.PriceAfterPromotion;
            existingOrder.AccountId = order.AccountId;
            existingOrder.ReasonCancel = order.ReasonCancel;

            //Sua sau
            return null;
        }
        public async Task<bool> DeleteByIdAsync(Guid id)
        {
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                return false;
            }
            _context.Orders.Remove(existingOrder);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
