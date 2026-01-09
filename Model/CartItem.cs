using System.ComponentModel; // INotifyPropertyChanged related types

namespace cashregister.Model // namespace for model types
{ // start namespace
    public class CartItem : INotifyPropertyChanged // model used to represent an entry in the shopping cart that notifies changes
    { // start class
        private int _quantity; // private backing field for Quantity property
        private decimal _discountPercent; // 0..1 fraction discount applied per item
        private bool _refunded; // legacy flag (kept for compatibility)
        private int _refundStatus; // 0 not refunded, 1 refunded, 2 refunddetails

        public string Name { get; set; } // the display name of the cart item
        public decimal Price { get; set; } // the unit price of the cart item

        public int Quantity // quantity of this item in the cart
        { // start property
            get => _quantity; // return the backing field
            set
            { // on set
                if (_quantity == value) return; // if no change, do nothing
                _quantity = value; // update backing field
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantity))); // notify that Quantity changed
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Subtotal))); // also notify that Subtotal changed
            } // end set
        } // end property

        public decimal DiscountPercent // 0..1 fraction (e.g., 0.10 = 10%)
        {
            get => _discountPercent;
            set
            {
                var normalized = value < 0 ? 0 : (value > 1 ? 1 : value);
                if (_discountPercent == normalized) return;
                _discountPercent = normalized;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DiscountPercent)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Subtotal)));
            }
        }

        // Legacy boolean; maps to refund status 1 when true, 0 when false
        public bool Refunded
        {
            get => _refunded;
            set
            {
                if (_refunded == value) return;
                _refunded = value;
                if (value) _refundStatus = 1; else if (_refundStatus == 1) _refundStatus = 0;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Refunded)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RefundStatus)));
            }
        }

        // 0 not refunded, 1 refunded, 2 refunddetails
        public int RefundStatus
        {
            get => _refundStatus;
            set
            {
                if (_refundStatus == value) return;
                _refundStatus = value;
                _refunded = (value == 1);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RefundStatus)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Refunded)));
            }
        }

        public decimal Subtotal => Price * Quantity * (1 - DiscountPercent); // computed subtotal with discount

        public event PropertyChangedEventHandler? PropertyChanged; // event implementation for INotifyPropertyChanged
    } // end class
} // end namespace
