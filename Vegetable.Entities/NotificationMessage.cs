using System;

namespace Vegetable.Entities
{
    public class NotificationMessage : BaseEntity
    {
        public string Title { get; set; }

        public string Text { get; set; }

        public DateTime NotificationDateUTC { get; set; }

        public int SendAttempts { get; set; }

        public DateTime? SentDateUTC { get; set; }

        public NotificationChannel Channel { get; set; }

        public string Recipient { get; set; }

        public string Platform { get; set; }

        public string RedirectUrl { get; set; }

        public string Result { get; set; }
    }

    public enum NotificationChannel
    {
        Push = 0,
        Telegram = 1,
        SMS = 2,
        TelegramInlineKeyboard = 3
    }
}
