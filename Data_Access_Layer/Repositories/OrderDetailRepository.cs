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
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartItemRepository _cartItemRepository;

        public OrderDetailRepository(ApplicationDbContext context, ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
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
                    query = query.Where(od => od.CartItemId.ToString().Contains(search));
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
            return await _context.OrderDetails
             .Include(od => od.CartItems)
             .Include(od => od.Orders)
             .FirstOrDefaultAsync(od => od.Id == id);
        }
       public async Task<OrderDetail?> UpdateAsync(Guid id, OrderDetail orderDetail)
        {
            var existingOrderDetail = await _context.OrderDetails.FindAsync(id);
            if (existingOrderDetail == null)
            {
                return null;
            }
            var cartItem = await _cartItemRepository.GetById(orderDetail.Id);
            //var ingredient = await _context.Ingredients.FirstOrDefaultAsync(i => i.Id == cartItem.IngredientId);
            //if (ingredient == null)
            //{
            //    throw new Exception("Ingredient not found.");
            //}
            existingOrderDetail.Quantity = cartItem.Quantity;
            existingOrderDetail.Price = cartItem.Price;
            existingOrderDetail.CartItemId = orderDetail.CartItemId;
            existingOrderDetail.OrderId = orderDetail.OrderId;
            _context.OrderDetails.Update(existingOrderDetail);
            await _context.SaveChangesAsync();
            return existingOrderDetail;
        }
    }
}
