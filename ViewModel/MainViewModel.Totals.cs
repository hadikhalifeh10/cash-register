using cashregister.Model;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Linq;

namespace cashregister.ViewModel
{
    public partial class MainViewModel
    {
        private void Cart_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var ni in e.NewItems.OfType<CartItem>())
                {
                    ni.PropertyChanged += CartItem_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (var oi in e.OldItems.OfType<CartItem>())
                {
                    oi.PropertyChanged -= CartItem_PropertyChanged;
                }
            }

            RaiseTotalsChanged();
        }

        private void CartItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CartItem.Quantity) || e.PropertyName == nameof(CartItem.Subtotal))
            {
                RaiseTotalsChanged();
            }
        }

        private void Checkout()
        {
            foreach (var ci in Cart.ToList())
            {
                ci.PropertyChanged -= CartItem_PropertyChanged;
            }

            Cart.Clear();
            RaiseTotalsChanged();
            StatusMessage = "Payment completed";
        }

        private void Cancel()
        {
            Checkout();
            IsPayPopupOpen = false;
            StatusMessage = "Transaction cancelled";
        }

        private void CommandManagerInvalidateRequerySuggested() => System.Windows.Input.CommandManager.InvalidateRequerySuggested();

        private void RaiseTotalsChanged()
        {
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(Gst));
            OnPropertyChanged(nameof(Qst));
            OnPropertyChanged(nameof(Total));
            CommandManagerInvalidateRequerySuggested();
        }

        public decimal Subtotal => Cart.Sum(c => c.Subtotal);
        public decimal Gst => Subtotal * 0.05m;
        public decimal Qst => Subtotal * 0.09975m;
    }
}
