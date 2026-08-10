using System;

namespace Vegetable.API.ViewModels.Payment
{
    public class InitResponse
    {
        public string TerminalKey { get; set; }
        public int Amount { get; set; }
        public Guid OrderId { get; set; }
        public bool Success { get; set; }
        public string Status { get; set; }
        public string PaymentId { get; set; }
        public string ErrorCode { get; set; }
        public string PaymentURL { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }

    }
}
