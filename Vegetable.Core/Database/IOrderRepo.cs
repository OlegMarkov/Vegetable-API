using System;
using System.Threading.Tasks;
using Vegetable.Entities;

namespace Vegetable.Core.Database
{
    public interface IOrderRepo
    {
        Task<bool> AddPaymentNotification(PaymentNotification paymentNotification);
        Task<Order> CreateOrder(Order order);
        Task<bool> UpdateOrder(Order order);
        Task<bool> DeleteOrder(Order order);
        Task<Order> GetPendingOrder(Guid ownerId, int subscriptionTypeId, int quantity);
    }
}
