using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vegetable.Entities
{
    public class Owner
    {
        public Guid Id { get; set; }

        [MaxLength(50)]
        public string UserId { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Alias { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        public bool AllowSite { get; set; }        

        public Currency Currency { get; set; }

        public ICollection<Employee> Employees { get; set; }

        public ICollection<Service> Services { get; set; }

        public ICollection<Reservation> Reservations { get; set; }

        public ICollection<Address> Addresses { get; set; }

        public ICollection<PhoneNumber> PhoneNumbers { get; set; }

        public ICollection<SocialNetwork> SocialNetworks { get; set; }

        public ICollection<Customer> Customers { get; set; }

        public ICollection<Schedule> Schedules { get; set; }

        public ICollection<ScheduleOnDay> ScheduleOnDays { get; set; }

        public ICollection<PaymentNotification> PaymentNotifications { get; set; }

        public ICollection<Image> Images { get; set; }

        public ICollection<User> Users { get; set; }

        public ICollection<Order> Orders { get; set; }

        public bool DisableReservationAtSameDay { get; set; }

        public DateTime? SubscriptionStartDate { get; set; }

        public DateTime? SubscriptionEndDate { get; set; }

        public int? SubscriptionTypeId { get; set; }

        public SubscriptionType SubscriptionType { get; set; }

        public string TimeZone { get; set; }

        public DateTime CreatedDateUTC { get; set; }

        public string Country { get; set; }

        public bool HasActiveSubscription => SubscriptionTypeId != null && SubscriptionEndDate > DateTime.UtcNow;
    }
}
