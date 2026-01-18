using cashregister.Model;
using System;
using System.Collections.Generic;

namespace cashregister.Common
{
    public class ReceiptRecord
    {
        public int Number { get; set; }
        public DateTime DateUtc { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Gst { get; set; }
        public decimal Qst { get; set; }
        public decimal TotalDiscountPercent { get; set; }
        public decimal Total { get; set; }
        public bool Refunded { get; set; }
        public bool IsRefund { get; set; }
        public int? RefundOfNumber { get; set; }
        public int RefundStatus { get; set; }
        public List<CartItem> Items { get; set; } = new();
    }
}
