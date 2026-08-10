using System.Collections.Generic;

namespace Vegetable.API.ViewModels.Payment
{
    public class PaymentNotificationMessage
    {
        public string TerminalKey { get; set; }
        public string OrderId { get; set; }
        public bool Success { get; set; }
        public string Status { get; set; }
        public ulong PaymentId { get; set; }
        public string ErrorCode { get; set; }
        public int Amount { get; set; }
        public ulong RebillId { get; set; }
        public ulong CardId { get; set; }
        public string Pan { get; set; }
        public string ExpDate { get; set; }
        public string Token { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}
