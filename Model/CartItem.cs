using System.ComponentModel;

namespace cashregister.Model
{
    public class CartItem : INotifyPropertyChanged
    {
        private int _quantity;

        public string Name { get; set; }
        public decimal Price { get; set; }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity == value) return;
                _quantity = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantity)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Subtotal)));
            }
        }

        public decimal Subtotal => Price * Quantity;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
