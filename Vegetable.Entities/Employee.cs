using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Vegetable.Entities
{
    public class Employee : BaseEntity
    {
        [MaxLength(20)]
        public string AuthId { get; set; }

        public Guid? AddressId { get; set; }

        public Address Address { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName { get { return $"{FirstName} {LastName}"; } }

        [NotMapped]
        public string Initials { 
            get {
                var fn = !string.IsNullOrEmpty(FirstName) ? FirstName.FirstOrDefault().ToString() : "";
                var ln = !string.IsNullOrEmpty(LastName) ? LastName.FirstOrDefault().ToString() : "";
                return fn + ln; 
            } 
        }

        [MaxLength(100)]
        public string Description { get; set; }

        public DateTime? StartOfWorkDate { get; set; }

        public DateTime? EndOfWorkDate { get; set; }

        public Days? WorkingDays { get; set; }

        public string Avatar { get; set; }

        [MaxLength(30)]
        public string Color { get; set; }

        public ICollection<Schedule> Schedules { get; set; }

        public ICollection<EmployeeService> EmployeeServices { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
        public ICollection<Image> Images { get; set; }
    }

    [Flags]
    public enum Days
    {
        None = 0x0,
        Sunday = 0x1,
        Monday = 0x2,
        Tuesday = 0x4,
        Wednesday = 0x8,
        Thursday = 0x10,
        Friday = 0x20,
        Saturday = 0x40
    }
}
