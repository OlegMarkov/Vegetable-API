using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vegetable.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public Owner Owner { get; set; }

        public Guid? OwnerId { get; set; }

        public DateTime CreatedDateUTC { get; set; }

        public bool Success { get; set; }

        [MaxLength(20)]
        public string Status { get; set; }

        public ulong PaymentId { get; set; }

        public string ErrorCode { get; set; }

        [MaxLength(100)]
        public string PaymentURL { get; set; }

        public string Message { get; set; }

        public int Amount { get; set; }

        public int Quantity { get; set; }

        public int SubscriptionTypeId { get; set; }

        public SubscriptionType SubscriptionType { get; set; }

        public ICollection<PaymentNotification> PaymentNotifications { get; set; }
    }
}
