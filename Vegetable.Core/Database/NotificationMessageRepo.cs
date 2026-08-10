using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vegetable.Entities;

namespace Vegetable.Core.Database
{
    public class NotificationMessageRepo : RepoBase, INotificationMessageRepo
    {
        public NotificationMessageRepo(PostgreDbContext postgreDbContext) : base(postgreDbContext) { }

        public async Task<NotificationMessage> CreateNotificationMessage(NotificationMessage notificationMessage)
        {
            try
            {
                _context.NotificationMessages.Add(notificationMessage);
                await _context.SaveChangesAsync();
                return notificationMessage;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteNotificationMessage(NotificationMessage notificationMessage)
        {
            try
            {
                _context.NotificationMessages.Remove(notificationMessage);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<NotificationMessage[]> GetNotificationMessagesForSending(int batchSize = 50)
        {
            try
            {
                return await _context.NotificationMessages.Where(x => x.NotificationDateUTC.Date <= DateTime.UtcNow.Date &&
                                                x.NotificationDateUTC.Date >= DateTime.UtcNow.AddDays(-14).Date &&
                                                x.SentDateUTC == null &&
                                                x.SendAttempts < 100).OrderBy(x => x.NotificationDateUTC).Take(batchSize).ToArrayAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> UpdateNotificationMessage(NotificationMessage notificationMessage)
        {
            try
            {
                _context.NotificationMessages.Update(notificationMessage);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
