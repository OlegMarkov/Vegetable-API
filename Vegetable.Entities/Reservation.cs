using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vegetable.Entities
{
    public class Reservation : BaseEntity
    {
        public Reservation()
        {
            Images = new List<Image>();
            ReservationServices = new List<ReservationService>();
        }

        public decimal Cost { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Guid? CustomerId { get; set; }
        public Customer Customer { get; set; }
        public Guid? EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public ICollection<Image> Images { get; set; }
        public ReservationType ReservationType { get; set; }
        public bool IsConfirmed { get; set; }
        public ICollection<ReservationService> ReservationServices { get; set; }
        public int RemindInMin { get; set; }
        public ICollection<Notification> Notifications { get; set; }

        [NotMapped]
        public bool IsExpired => StartTime < DateTime.UtcNow;
    }

    public enum ReservationType
    {
        OwnerApp = 0,
        CustomerWeb = 1
    }
}
