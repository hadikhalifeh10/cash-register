using System.ComponentModel; // INotifyPropertyChanged related types

namespace cashregister.Model // namespace for model types
{ // start namespace
    public class CartItem : INotifyPropertyChanged // model used to represent an entry in the shopping cart that notifies changes
    { // start class
        private int _quantity; // private backing field for Quantity property

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

        public decimal Subtotal => Price * Quantity; // computed subtotal based on price and quantity

        public event PropertyChangedEventHandler? PropertyChanged; // event implementation for INotifyPropertyChanged
    } // end class
} // end namespace
