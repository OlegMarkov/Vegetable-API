using System;
using System.ComponentModel.DataAnnotations;

namespace Vegetable.Entities
{

    public class Image : BaseEntity
    {
        [MaxLength(150)]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public bool IsPrimary { get; set; }

        public Guid? ReservationId { get; set; }

        public Reservation Reservation { get; set; }

        public Guid? CustomerId { get; set; }

        public Customer Customer { get; set; }

        public Guid? ServiceId { get; set; }

        public Service Service { get; set; }

        public Guid? EmployeeId { get; set; }

        public Employee Employee { get; set; }
    }
}
