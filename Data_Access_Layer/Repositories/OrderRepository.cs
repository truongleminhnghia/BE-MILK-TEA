using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace Data_Access_Layer.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Order?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.Orders.Include(o => o.OrderDetails)
                                            .Include(o => o.Payments)
                                            .Include(o => o.OrderPromotions)
                                            .FirstOrDefaultAsync(o => o.Id == id);
            }
            catch (Exception ex)
            {
                // Log the exception (if using logging)
                Console.WriteLine($"loi o GetByIdAsync: {ex.Message}");
                return null;
            }
        }
        public async Task<Order> CreateAsync(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return order;
            }
            catch (Exception ex)
            {
                // Log the exception (if using logging)
                Console.WriteLine($"Lỗi tạo Order: {ex.Message}");
                return null;
            }
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            var existingOrder = await _context.Orders.FindAsync(order.Id);
            if (existingOrder == null)
            {
                return null;
            }
            existingOrder.Quantity = order.Quantity;
            //existingOrder.PriceAfterPromotion = order.PriceAfterPromotion;
            existingOrder.TotalPrice = order.TotalPrice;
            existingOrder.ReasonCancel = order.ReasonCancel;

            await _context.SaveChangesAsync();
            return existingOrder;
        }
        //public async Task<bool> DeleteByIdAsync(Guid id)
        //{
        //    var existingOrder = await _context.Orders.FindAsync(id);
        //    if (existingOrder == null)
        //    {
        //        return false;
        //    }

        //    existingOrder.OrderStatus =OrderStatus.CONFIRM;
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        public async Task<List<Order>> GetAllOrdersAsync(Guid accountId, string? search, string? sortBy, bool isDescending, OrderStatus? orderStatus, DateTime? orderDate, int page, int pageSize)
        {
            try
            {

                var baseQuery = _context.OrderDetails.Where(w => w.Orders.AccountId == accountId).Include(ic => ic.Orders).AsQueryable();
                var query = baseQuery.Select(o => new Order
                {
                    Id = o.Id,
                    OrderCode = o.Orders.OrderCode,
                    OrderDate = o.Orders.OrderDate,
                    FullNameShipping = o.Orders.FullNameShipping,
                    PhoneShipping = o.Orders.PhoneShipping,
                    EmailShipping = o.Orders.EmailShipping,
                    NoteShipping = o.Orders.NoteShipping,
                    Quantity = o.Orders.Quantity,
                    TotalPrice = o.Orders.TotalPrice,
                    PriceAffterPromotion = o.Orders.PriceAffterPromotion,
                    AddressShipping = o.Orders.AddressShipping,
                    OrderDetails = o.Orders.OrderDetails 
                }).AsQueryable();
                

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(o => o.OrderCode.ToLower().Contains(search.ToLower()));
                }

                if (orderStatus.HasValue)
                {
                    query = query.Where(o => o.OrderStatus == orderStatus.Value);
                }

                if (orderDate.HasValue)
                {
                    query = query.Where(o => o.OrderDate.Date == orderDate.Value.Date);
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
                throw new Exception($"Không thể lọc được order: {ex.Message}", ex);
            }
        }

        public async Task<Order?> UpdateStatusAsync(Guid id, Order order)
        {
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                return null;
            }

            bool isRefCodeExists = await _context.Employees.AnyAsync(e => e.RefCode == order.RefCode);
            if (!isRefCodeExists)
            {
                throw new Exception("RefCode không tồn tại trong bảng Employee.");
            }

            existingOrder.RefCode = order.RefCode;
            existingOrder.ReasonCancel = order.ReasonCancel;
            existingOrder.OrderStatus = order.OrderStatus;

            await _context.SaveChangesAsync();
            return existingOrder;
        }

        public async Task<Order?> GetByCodeAsync(string id)
        {
            try
            {
                return await _context.Orders.Include(o => o.OrderDetails)
                                            .Include(o => o.Payments)
                                            .Include(o => o.OrderPromotions)
                                            .FirstOrDefaultAsync(o => o.OrderCode == id);
            }
            catch (Exception ex)
            {
                // Log the exception (if using logging)
                Console.WriteLine($"loi o GetByIdAsync: {ex.Message}");
                return null;
            }
        }
    }
}
