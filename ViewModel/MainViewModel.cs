using cashregister.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace cashregister.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Item> Items { get; } = new();
        public ObservableCollection<CartItem> Cart { get; } = new();

        public RelayCommand AddToCartCommand { get; }
        public RelayCommand RemoveFromCartCommand { get; }
        public RelayCommand CheckoutCommand { get; }

        // New individual add commands
        public RelayCommand AddItem1 { get; }
        public RelayCommand AddItem2 { get; }
        public RelayCommand AddItem3 { get; }
        public RelayCommand AddItem4 { get; }
        public RelayCommand AddItem5 { get; }

        public MainViewModel()
        {
            // sample items - ensure five items exist
            Items.Add(new Item { Name = "Apple", Price = 0.99m });
            Items.Add(new Item { Name = "Banana", Price = 0.59m });
            Items.Add(new Item { Name = "Chocolate", Price = 2.49m });
            Items.Add(new Item { Name = "Bread", Price = 1.99m });
            Items.Add(new Item { Name = "Milk", Price = 1.49m });

            AddToCartCommand = new RelayCommand(p => AddToCart(p as Item));
            RemoveFromCartCommand = new RelayCommand(p => RemoveFromCart(p as CartItem), p => p is CartItem);
            CheckoutCommand = new RelayCommand(_ => Checkout(), _ => Cart.Any());

            // individual item commands call AddToCart with the corresponding item
            AddItem1 = new RelayCommand(_ => AddToCartAtIndex(0));
            AddItem2 = new RelayCommand(_ => AddToCartAtIndex(1));
            AddItem3 = new RelayCommand(_ => AddToCartAtIndex(2));
            AddItem4 = new RelayCommand(_ => AddToCartAtIndex(3));
            AddItem5 = new RelayCommand(_ => AddToCartAtIndex(4));

            Cart.CollectionChanged += (s, e) =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Total)));
                CommandManagerInvalidateRequerySuggested();
            };
        }

        private void CommandManagerInvalidateRequerySuggested() => System.Windows.Input.CommandManager.InvalidateRequerySuggested();

        private void AddToCartAtIndex(int index)
        {
            Item? item = null;
            if (index >= 0 && index < Items.Count) item = Items[index];
            AddToCart(item);
        }

        private void AddToCart(Item? item)
        {
            if (item == null) return;
            var existing = Cart.FirstOrDefault(c => c.Name == item.Name);
            if (existing != null)
            {
                existing.Quantity++;
                // existing will raise property changed
            }
            else
            {
                Cart.Add(new CartItem { Name = item.Name, Price = item.Price, Quantity = 1 });
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Total)));
            CommandManagerInvalidateRequerySuggested();
        }

        private void RemoveFromCart(CartItem? cartItem)
        {
            if (cartItem == null) return;
            cartItem.Quantity--;
            if (cartItem.Quantity <= 0)
            {
                Cart.Remove(cartItem);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Total)));
            CommandManagerInvalidateRequerySuggested();
        }

        private void Checkout()
        {
            Cart.Clear();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Total)));
            CommandManagerInvalidateRequerySuggested();
        }

        public decimal Total => Cart.Sum(c => c.Subtotal);

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
