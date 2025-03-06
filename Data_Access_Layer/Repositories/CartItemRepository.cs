using System.Collections.Generic;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Data_Access_Layer;
using Microsoft.EntityFrameworkCore;
using Business_Logic_Layer.Repositories;
using Data_Access_Layer.Repositories;
using System.Linq.Expressions;
using System.Net;



namespace Business_Logic_Layer.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly ApplicationDbContext _context;

        public CartItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public CartItemRepository()
        {
        }

        public async Task<IEnumerable<CartItem>> GetAllAsync()
        {
            try
            {
                var cartItems = await _context.CartItems.ToListAsync();
                return cartItems; // Không cần Ok()
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách CartItem", ex);
            }
        }
        

        public async Task<CartItem> GetByIdAsync(Guid id)
        {
            try
            {
                var cartItem = await _context.CartItems.FindAsync(id);
                if (cartItem == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy CartItem với ID {id}");
                }
                return cartItem;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy CartItem với ID {id}", ex);
            }
        }

        public async Task AddAsync(CartItem cartItem)
        {
            try
            {
                await _context.CartItems.AddAsync(cartItem);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm CartItem", ex);
            }
        }

        public async Task UpdateAsync(CartItem cartItem)
        {
            try
            {
                var existingCartItem = await _context.CartItems.FindAsync(cartItem.Id);
                if (existingCartItem == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy CartItem với ID {cartItem.Id} để cập nhật");
                }

                _context.Entry(existingCartItem).CurrentValues.SetValues(cartItem);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật CartItem với ID {cartItem.Id}", ex);

            }
        }
        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var cartItem = await _context.CartItems.FindAsync(id);
                if (cartItem != null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy CartItem với ID {id} để xóa");
                }

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa CartItem với ID {id}", ex);
            }
        }
   
        public Task<IEnumerable<CartItem>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<CartItem> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task Add(CartItem cartItem)
        {
            throw new NotImplementedException();
        }

        public Task<CartItem> Update(CartItem cartItem)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<CartItem> Create(CartItem cartItem)
        {
            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }
    }
}
