using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> GetByIdAsync(Guid id)
        {
            return await _context
                .Payments.Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Payment>> GetAllAsync()
        {
            return await _context.Payments.Include(p => p.Order).ToListAsync();
        }

        public async Task<List<Payment>> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.Payments.Where(p => p.OrderId == orderId).ToListAsync();
        }

        public async Task<Payment> CreateAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> UpdateAsync(Payment payment)
        {
            var existingPayment = await GetByIdAsync(payment.Id);
            if (existingPayment != null)
            {
                _context.Entry(existingPayment).State = EntityState.Detached;
            }
            _context.Entry(payment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return payment;
        }
    }
}
