using cashregister.Model; // reference the Item and CartItem model types
using System; // basic types
using System.Collections.ObjectModel; // use ObservableCollection for collections bound to the UI
using System.ComponentModel; // INotifyPropertyChanged interface and event args
using System.Linq; // LINQ methods such as FirstOrDefault and Sum
using System.Collections.Specialized; // NotifyCollectionChangedEventArgs

namespace cashregister.ViewModel // namespace for view models
{ // start namespace
    public class MainViewModel : INotifyPropertyChanged // view model that notifies view about property changes
    { // start class
        public ObservableCollection<Item> Items { get; } = new(); // collection of available items in the store
        public ObservableCollection<CartItem> Cart { get; } = new(); // collection representing the shopping cart

        public ObservableCollection<CommandButton> Buttons { get; } = new(); // dynamic collection of buttons exposed to the view

        public RelayCommand AddToCartCommand { get; } // command to add a selected Item to the cart
        public RelayCommand RemoveFromCartCommand { get; } // command to remove or decrement a CartItem from the cart
        public RelayCommand CheckoutCommand { get; } // command to perform checkout (clear the cart)
        public RelayCommand CancelCommand { get; } // command to cancel and reset the register

        public RelayCommand PayCreditCommand { get; } // pay via credit
        public RelayCommand PayDebitCommand { get; } // pay via debit
        public RelayCommand PayCashCommand { get; } // pay via cash

        public RelayCommand AddNewItemCommand { get; } // command to add a new item to Items from UI

        private bool _isPayPopupOpen; // backing field for whether the pay popup is open
        public bool IsPayPopupOpen // whether the pay options popup is open
        {
            get => _isPayPopupOpen; // return current state
            set
            {
                if (_isPayPopupOpen == value) return; // no change
                _isPayPopupOpen = value; // update
                OnPropertyChanged(nameof(IsPayPopupOpen)); // notify UI
            }
        }

        private string _newItemName = string.Empty; // backing for new item name text box
        public string NewItemName // bound to UI textbox for new item name
        {
            get => _newItemName;
            set
            {
                if (_newItemName == value) return;
                _newItemName = value;
                OnPropertyChanged(nameof(NewItemName));
            }
        }

        private string _newItemPriceText = string.Empty; // backing for new item price input
        public string NewItemPriceText // bound to UI textbox for new item price
        {
            get => _newItemPriceText;
            set
            {
                if (_newItemPriceText == value) return;
                _newItemPriceText = value;
                OnPropertyChanged(nameof(NewItemPriceText));
            }
        }

        private string _statusMessage = string.Empty; // small unobtrusive status message shown in UI
        public string StatusMessage // bound to the status bar TextBlock
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage == value) return;
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        public MainViewModel() // constructor initializes collections and commands
        { // start ctor
            // sample items - ensure five items exist
            Items.Add(new Item { Name = "Apple", Price = 0.99m }); // add Apple item to Items collection
            Items.Add(new Item { Name = "Banana", Price = 0.59m }); // add Banana item to Items collection
            Items.Add(new Item { Name = "Chocolate", Price = 2.49m }); // add Chocolate item to Items collection
            Items.Add(new Item { Name = "Bread", Price = 1.99m }); // add Bread item to Items collection
            Items.Add(new Item { Name = "Milk", Price = 1.49m }); // add Milk item to Items collection
            Items.Add(new Item { Name = "Eggs", Price = 2.99m }); // add Eggs item to Items collection

            AddToCartCommand = new RelayCommand(p => AddToCart(p as Item)); // create command that calls AddToCart with parameter cast to Item
            RemoveFromCartCommand = new RelayCommand(p => RemoveFromCart(p as CartItem), p => p is CartItem); // create command to remove cart item with canExecute checking parameter is CartItem
            CheckoutCommand = new RelayCommand(_ => Checkout(), _ => Cart.Any()); // create checkout command that only can execute when cart has items
            CancelCommand = new RelayCommand(_ => Cancel(), _ => true); // cancel always enabled

            // pay commands clear the cart and close the popup
            PayCreditCommand = new RelayCommand(_ => PayAndClose());
            PayDebitCommand = new RelayCommand(_ => PayAndClose());
            PayCashCommand = new RelayCommand(_ => PayAndClose());

            AddNewItemCommand = new RelayCommand(_ => AddNewItem()); // add new item from UI

            // Build Buttons dynamically from Items collection so any button added in the viewmodel shows up in the view
            foreach (var item in Items)
            {
                Buttons.Add(CreateButtonForItem(item));
            }

            // subscribe to changes in Items so newly added items also get buttons
            Items.CollectionChanged += Items_CollectionChanged;

            // subscribe to collection changes to track item additions/removals and update totals
            Cart.CollectionChanged += Cart_CollectionChanged;
        } // end ctor

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var ni in e.NewItems.OfType<Item>())
                {
                    Buttons.Add(CreateButtonForItem(ni));
                }
            }

            if (e.OldItems != null)
            {
                foreach (var oi in e.OldItems.OfType<Item>())
                {
                    var btn = Buttons.FirstOrDefault(b => b.Label.StartsWith($"Add {oi.Name} "));
                    if (btn != null) Buttons.Remove(btn);
                }
            }
        }

        private CommandButton CreateButtonForItem(Item item)
        {
            // create CommandButton with label and command to add this item
            return new CommandButton { Label = $"Add {item.Name} - {item.Price:C}", Command = new RelayCommand(_ => AddToCart(item)) };
        }

        private void AddNewItem()
        {
            // try parse price
            if (string.IsNullOrWhiteSpace(NewItemName))
            {
                StatusMessage = "Item name required";
                return;
            }

            if (!decimal.TryParse(NewItemPriceText, out var price))
            {
                StatusMessage = "Invalid price";
                return;
            }

            var newItem = new Item { Name = NewItemName.Trim(), Price = price };
            Items.Add(newItem);
            StatusMessage = $"{newItem.Name} added successfully"; // unobtrusive status

            // clear inputs
            NewItemName = string.Empty;
            NewItemPriceText = string.Empty;
        }

        private void Cart_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // subscribe to PropertyChanged on new items so quantity changes update totals
            if (e.NewItems != null)
            {
                foreach (var ni in e.NewItems.OfType<CartItem>())
                {
                    ni.PropertyChanged += CartItem_PropertyChanged;
                }
            }

            // unsubscribe from old items
            if (e.OldItems != null)
            {
                foreach (var oi in e.OldItems.OfType<CartItem>())
                {
                    oi.PropertyChanged -= CartItem_PropertyChanged;
                }
            }

            RaiseTotalsChanged(); // whenever collection changes, update subtotal/gst/qst/total bindings and commands
        }

        private void CartItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // when a CartItem's Quantity or Subtotal changes, update totals
            if (e.PropertyName == nameof(CartItem.Quantity) || e.PropertyName == nameof(CartItem.Subtotal))
            {
                RaiseTotalsChanged();
            }
        }

        private void CommandManagerInvalidateRequerySuggested() => System.Windows.Input.CommandManager.InvalidateRequerySuggested(); // helper to force command requery

        private void RaiseTotalsChanged()
        {
            OnPropertyChanged(nameof(Subtotal)); // notify subtotal changed
            OnPropertyChanged(nameof(Gst)); // notify GST changed
            OnPropertyChanged(nameof(Qst)); // notify QST changed
            OnPropertyChanged(nameof(Total)); // notify final total changed
            CommandManagerInvalidateRequerySuggested(); // re-evaluate command CanExecute states
        }

        private void AddToCartAtIndex(int index) // helper that retrieves an Item by index and forwards to AddToCart
        { // start method
            Item? item = null; // local variable to hold the resolved item
            if (index >= 0 && index < Items.Count) item = Items[index]; // if index valid, get the item from Items
            AddToCart(item); // add the resolved item to the cart (or no-op if null)
        } // end method

        private void AddToCart(Item? item) // core logic to add an item into the cart
        { // start method
            if (item == null) return; // do nothing when item is null
            var existing = Cart.FirstOrDefault(c => c.Name == item.Name); // find existing cart entry with same name
            if (existing != null)
            {
                existing.Quantity++; // increment quantity when item already in cart
                // existing will raise property changed which we handle in CartItem_PropertyChanged
            }
            else
            {
                var cartItem = new CartItem { Name = item.Name, Price = item.Price, Quantity = 1 }; // create new cart item
                Cart.Add(cartItem); // add to observable collection; CollectionChanged handler will subscribe to its PropertyChanged
            }

            // set unobtrusive status message for UI
            StatusMessage = $"{item.Name} added successfully";
        } // end method

        private void RemoveFromCart(CartItem? cartItem) // logic to decrement or remove a cart item
        { // start method
            if (cartItem == null) return; // no-op if parameter null
            cartItem.Quantity--; // decrement the quantity
            if (cartItem.Quantity <= 0)
            {
                // unsubscribe then remove
                cartItem.PropertyChanged -= CartItem_PropertyChanged;
                Cart.Remove(cartItem); // remove from collection when quantity reaches zero or below
            }

            // totals updated via PropertyChanged or CollectionChanged
            StatusMessage = cartItem != null ? $"{cartItem.Name} removed" : StatusMessage;
        } // end method

        private void Checkout() // clear the cart and update observers
        { // start method
            // unsubscribe from all items to avoid memory leaks
            foreach (var ci in Cart.ToList())
            {
                ci.PropertyChanged -= CartItem_PropertyChanged;
            }

            Cart.Clear(); // remove all items from cart
            RaiseTotalsChanged(); // notify total changed after clearing
            StatusMessage = "Payment completed"; // unobtrusive status
        } // end method

        private void Cancel() // cancel resets register and closes pay popup
        {
            Checkout(); // clear the cart
            IsPayPopupOpen = false; // close popup if open
            StatusMessage = "Transaction cancelled";
        }

        private void PayAndClose() // handle a payment option selection
        {
            Checkout(); // clear the cart as payment completes
            IsPayPopupOpen = false; // close popup
        }

        public decimal Subtotal => Cart.Sum(c => c.Subtotal); // computed property that sums subtotals of items in cart

        public decimal Gst => Subtotal * 0.05m; // GST at 5% of subtotal

        public decimal Qst => Subtotal * 0.09975m; // QST at 9.975% of subtotal

        public decimal Total => Subtotal + Gst + Qst; // final total including taxes

        public event PropertyChangedEventHandler? PropertyChanged; // event used by INotifyPropertyChanged to notify the view

        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); // helper to raise PropertyChanged
    } // end class
} // end namespace
