using System;
using System.ComponentModel.DataAnnotations;

namespace Vegetable.Entities
{
    public class PaymentNotification
    {
        public Guid Id { get; set; }
        public Owner Owner { get; set; }
        public Guid? OwnerId { get; set; }
        public DateTime CreatedDateUTC { get; set; }
        [MaxLength(20)]
        public string TerminalKey { get; set; }
        public Guid OrderId { get; set; }
        public bool Success { get; set; }
        [MaxLength(20)]
        public string Status { get; set; }
        public ulong PaymentId { get; set; }
        [MaxLength(20)]
        public string ErrorCode { get; set; }
        public int Amount { get; set; }
        public ulong RebillId { get; set; }
        public ulong CardId { get; set; }
        public string Pan { get; set; }
        public string ExpDate { get; set; }
        public string Token { get; set; }
        public string Data { get; set; }
        public Order Order { get; set; }
    }

    
}
