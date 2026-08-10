using System;

namespace Vegetable.Entities
{
    public class Notification : BaseEntity
    {        
        public string Title { get; set; }

        public string Description { get; set; }

        public string Note { get; set; }

        public NotificationType NotificationType { get; set; }

        public DateTime NotificationDateUTC { get; set; }

        public int SendAttempts { get; set; }

        public DateTime? SentDateUTC { get; set; }

        public Guid? ReservationId { get; set; }

        public Reservation Reservation { get; set; }

        public Guid? CustomerId { get; set; }

        public Customer Customer { get; set; }

        public Guid? ServiceId { get; set; }

        public Service Service { get; set; }

        public Guid? ScheduleId { get; set; }

        public Schedule Schedule { get; set; }

        public Guid? EmployeeId { get; set; }

        public Employee Employee { get; set; }
    }

    public enum NotificationType
    {
        DailyReport = 0,
        NewReservationClient = 1,
        CancelReservationClient = 2,
        ChangeReservationClient = 3,
        NewReservationOwner = 4,
        CancelReservationOwner = 5,
        ChangeReservationOwner = 6,
        ReminderReservation = 7,
        SubscriptionCreated = 8,
        ReminderSubscriptionEnd = 9,
        SubscriptionEdnded = 10,
        ReminderClientBirthday = 11,
        ConfirmationReservationOwner = 12,
        ConfirmationReservationClient = 13
    }
}
