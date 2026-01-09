using System;
using System.Collections.Generic;
using cashregister.Model;

namespace cashregister.ViewModel
{
    // Simple DTO representing a receipt header and items for history display
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
        public int RefundStatus { get; set; } // 0 not refunded, 1 refunded, 2 refund details
        public List<CartItem> Items { get; set; } = new();
    }
}
