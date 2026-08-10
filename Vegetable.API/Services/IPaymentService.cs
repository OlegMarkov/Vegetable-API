using System.Threading.Tasks;
using Vegetable.API.ViewModels.Payment;
using Vegetable.Entities;

namespace Vegetable.API.Services
{
    public interface IPaymentService
    {
        bool CheckSign(PaymentNotificationMessage paymentNotification);
        Task<InitResponse> InitPaymentRequest(Order order, Owner owner, SubscriptionType subscriptionType);
    }
}
