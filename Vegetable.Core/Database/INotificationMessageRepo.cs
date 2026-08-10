using System.Threading.Tasks;
using Vegetable.Entities;

namespace Vegetable.Core.Database
{
    public interface INotificationMessageRepo
    {
        Task<NotificationMessage> CreateNotificationMessage(NotificationMessage notificationMessage);
        Task<bool> UpdateNotificationMessage(NotificationMessage notificationMessage);
        Task<bool> DeleteNotificationMessage(NotificationMessage notificationMessage);
        Task<NotificationMessage[]> GetNotificationMessagesForSending(int batchSize = 50);
    }
}
