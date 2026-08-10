using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vegetable.Entities;

namespace Vegetable.Core.Database
{
    public class OrderRepo : RepoBase, IOrderRepo
    {
        public OrderRepo(PostgreDbContext postgreDbContext) : base(postgreDbContext) { }

        public async Task<bool> AddPaymentNotification(PaymentNotification paymentNotification)
        {
            try
            {
                var order = await _context.Orders.Where(x => x.Id == paymentNotification.OrderId).FirstOrDefaultAsync();
                if (order == null) throw new Exception("Order not found");
                order.Status = paymentNotification.Status;
                order.Amount = paymentNotification.Amount;
                paymentNotification.OwnerId = order.OwnerId;
                _context.Attach(paymentNotification);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Order> CreateOrder(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return order;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> UpdateOrder(Order order)
        {
            try
            {
                _context.Orders.Attach(order);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Order> GetPendingOrder(Guid ownerId, int subscriptionTypeId, int quantity)
        {
            try
            {
                return await _context.Orders.FirstOrDefaultAsync(x => 
                    x.OwnerId == ownerId
                    && x.SubscriptionTypeId == subscriptionTypeId
                    && x.Quantity == quantity
                    && x.Status == "NEW"
                    && x.CreatedDateUTC > DateTime.UtcNow.AddHours(-23)
                    && x.PaymentURL != null); 
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteOrder(Order order)
        {
            try
            {
                _context.Orders.Remove(order);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
