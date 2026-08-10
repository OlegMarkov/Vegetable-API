using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vegetable.Entities
{
    public class Service : BaseEntity
    {
        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        /// <summary>
        /// Duration of service in minutes
        /// </summary>
        public double DurationInMinutes { get; set; }

        public decimal Cost { get; set; }

        [MaxLength(3)]
        public string CurrencyCode { get; set; }

        public short? UsersCount { get; set; }

        [MaxLength(30)]
        public string Color { get; set; }

        [NotMapped]
        public TimeSpan Duration { get { return TimeSpan.FromMinutes(DurationInMinutes); } }

        public ICollection<EmployeeService> EmployeeServices { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Image> Images { get; set; }

        public ICollection<ReservationService> ReservationServices { get; set; }
    }

    public class ServiceComparer : IEqualityComparer<Service>
    {
        public bool Equals(Service x, Service y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the Schedule' properties are equal.
            return x.Title == y.Title && x.Cost == y.Cost;
        }

        public int GetHashCode(Service schedule)
        {
            if (Object.ReferenceEquals(schedule, null)) return 0;
            int hashProductName = schedule.Title == null ? 0 : schedule.Title.GetHashCode();
            int hashProductCode = schedule.Cost.GetHashCode();
            return hashProductName ^ hashProductCode;
        }
    }
}
