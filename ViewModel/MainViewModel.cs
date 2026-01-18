using cashregister.Common;
using cashregister.Model;
using cashregister.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Windows;

namespace cashregister.ViewModel
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Item> Items { get; } = new();
        public ObservableCollection<CartItem> Cart { get; } = new();

        public ObservableCollection<CommandButton> Buttons { get; } = new();
        public ObservableCollection<string> Categories { get; } = new();

        private string _selectedCategory = "base";
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory == value) return;
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                RebuildButtonsForSelectedCategory();
            }
        }

        public RelayCommand AddToCartCommand { get; }
        public RelayCommand RemoveFromCartCommand { get; }
        public RelayCommand CheckoutCommand { get; }
        public RelayCommand CancelCommand { get; }

        public RelayCommand PayCreditCommand { get; }
        public RelayCommand PayDebitCommand { get; }
        public RelayCommand PayCashCommand { get; }

        public RelayCommand AddNewItemCommand { get; }
        public RelayCommand ApplyItemDiscountCommand { get; }
        public RelayCommand ApplyTotalDiscountCommand { get; }
        public RelayCommand LoadReceiptCommand { get; }

        // Add Close/Reset commands
        public RelayCommand CloseAppCommand { get; }
        public RelayCommand FactoryResetCommand { get; }

        private bool _isPayPopupOpen;
        public bool IsPayPopupOpen
        {
            get => _isPayPopupOpen;
            set
            {
                if (_isPayPopupOpen == value) return;
                _isPayPopupOpen = value;
                OnPropertyChanged(nameof(IsPayPopupOpen));
            }
        }

        private string _newItemName = string.Empty;
        public string NewItemName
        {
            get => _newItemName;
            set { if (_newItemName == value) return; _newItemName = value; OnPropertyChanged(nameof(NewItemName)); }
        }

        private string _newItemPriceText = string.Empty;
        public string NewItemPriceText
        {
            get => _newItemPriceText;
            set { if (_newItemPriceText == value) return; _newItemPriceText = value; OnPropertyChanged(nameof(NewItemPriceText)); }
        }

        private string _newItemCategory = string.Empty;
        public string NewItemCategory
        {
            get => _newItemCategory;
            set { if (_newItemCategory == value) return; _newItemCategory = value; OnPropertyChanged(nameof(NewItemCategory)); }
        }

        private string _discountText = string.Empty;
        public string DiscountText
        {
            get => _discountText;
            set { if (_discountText == value) return; _discountText = value; OnPropertyChanged(nameof(DiscountText)); }
        }

        private string _loadReceiptNumber = string.Empty;
        public string LoadReceiptNumber
        {
            get => _loadReceiptNumber;
            set { if (_loadReceiptNumber == value) return; _loadReceiptNumber = value; OnPropertyChanged(nameof(LoadReceiptNumber)); }
        }

        private decimal _totalDiscountPercent;
        public decimal TotalDiscountPercent
        {
            get => _totalDiscountPercent;
            set { var normalized = value < 0 ? 0 : (value > 1 ? 1 : value); if (_totalDiscountPercent == normalized) return; _totalDiscountPercent = normalized; OnPropertyChanged(nameof(TotalDiscountPercent)); RaiseTotalsChanged(); }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set { if (_statusMessage == value) return; _statusMessage = value; OnPropertyChanged(nameof(StatusMessage)); }
        }

        private readonly ReceiptService _receiptService = new();
        private readonly ConfigService _configService = new();
        private readonly cashregister.Services.KeyboardService _keyboard = new();

        public MainViewModel()
        {
            // Initialize Commands
            CloseAppCommand = new RelayCommand(_ => CloseApp());
            FactoryResetCommand = new RelayCommand(_ => PerformFactoryReset());

            Items.CollectionChanged += Items_CollectionChanged;
            Cart.CollectionChanged += Cart_CollectionChanged;

            // Load Inventory
            LoadInventory();

            Categories.Add("base");
            SelectedCategory = "base";

            AddToCartCommand = new RelayCommand(p => AddToCart(p as Item));
            RemoveFromCartCommand = new RelayCommand(p => RemoveFromCart(p as CartItem), p => p is CartItem);
            CheckoutCommand = new RelayCommand(_ => Checkout(), _ => Cart.Any());
            CancelCommand = new RelayCommand(_ => Cancel(), _ => true);

            PayCreditCommand = new RelayCommand(_ => PayAndCloseWithReceipt("Credit"));
            PayDebitCommand = new RelayCommand(_ => PayAndCloseWithReceipt("Debit"));
            PayCashCommand = new RelayCommand(_ => PayAndCloseWithReceipt("Cash"));

            AddNewItemCommand = new RelayCommand(_ => AddNewItem());
            ApplyItemDiscountCommand = new RelayCommand(p => ApplyItemDiscount(p as CartItem));
            ApplyTotalDiscountCommand = new RelayCommand(_ => ApplyTotalDiscount());
            LoadReceiptCommand = new RelayCommand(_ => LoadReceipt());

            RebuildButtonsForSelectedCategory();
            
            EnsureReceiptsFolder();
            InitializeCashTendering();
            InitializeKeyboard();
            InitializeSplit();

            // Load App State (Cart)
            _configService.LoadState(Cart);
        }

        private void EnsureReceiptsFolder() => _receiptService.EnsureReceiptsFolder();
        private int GetNextReceiptNumber() => _receiptService.GetNextReceiptNumber();
        private void SaveReceipt(int number, string paymentMethod) => _receiptService.SaveReceipt(number, paymentMethod, Cart, Subtotal, Gst, Qst, TotalDiscountPercent, Total);

        private bool TryLoadReceipt(int number)
        {
            var ok = _receiptService.TryLoadReceipt(number, Cart, out var totalDiscount);
            if (ok) TotalDiscountPercent = totalDiscount;
            return ok;
        }

        private void LoadReceipt()
        {
            if (!int.TryParse(LoadReceiptNumber, out var number) || number <= 0)
            { StatusMessage = "Enter a valid receipt number"; return; }
            if (!TryLoadReceipt(number)) { StatusMessage = $"Receipt {number} not found"; }
        }

        private void LoadInventory()
        {
            Items.Clear();
            var dbItems = _receiptService.LoadInventory();
            if (dbItems.Any())
            {
                foreach (var i in dbItems) Items.Add(i);
            }
            else
            {
                // Seed defaults
                var defaults = new[]
                {
                    new Item { Name = "Apple", Price = 0.99m, Category = "base" },
                    new Item { Name = "Banana", Price = 0.59m, Category = "base" },
                    new Item { Name = "Chocolate", Price = 2.49m, Category = "base" },
                    new Item { Name = "Bread", Price = 1.99m, Category = "base" },
                    new Item { Name = "Milk", Price = 1.49m, Category = "base" },
                    new Item { Name = "Eggs", Price = 2.99m, Category = "base" }
                };
                foreach (var d in defaults)
                {
                    _receiptService.SaveInventoryItem(d); // Save grabs ID
                    Items.Add(d);
                }
            }
        }

        private void CloseApp()
        {
            _configService.SaveState(Cart);
            Application.Current.Shutdown();
        }

        private void PerformFactoryReset()
        {
            if (MessageBox.Show("Are you sure you want to delete all data (receipts, inventory, settings)? This cannot be undone.", "Factory Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _receiptService.FactoryReset();
                _configService.ClearState();
                Cart.Clear();
                LoadInventory(); // re-seed defaults
                StatusMessage = "Factory reset complete.";
            }
        }

        private bool _isCashTenderVisible;
        public bool IsCashTenderVisible
        {
            get => _isCashTenderVisible;
            set { if (_isCashTenderVisible == value) return; _isCashTenderVisible = value; OnPropertyChanged(nameof(IsCashTenderVisible)); }
        }

        private string _cashTenderText = string.Empty;
        public string CashTenderText
        {
            get => _cashTenderText;
            set
            {
                if (_cashTenderText == value) return;
                _cashTenderText = value;
                OnPropertyChanged(nameof(CashTenderText));
                UpdateCashTenderCalculations();
            }
        }

        private decimal _cashTenderAmount;
        public decimal CashTenderAmount
        {
            get => _cashTenderAmount;
            private set { if (_cashTenderAmount == value) return; _cashTenderAmount = value; OnPropertyChanged(nameof(CashTenderAmount)); OnPropertyChanged(nameof(CashChange)); OnPropertyChanged(nameof(CashDue)); }
        }

        public decimal CashChange => CashTenderAmount > CashTotalRounded ? CashTenderAmount - CashTotalRounded : 0m;
        public decimal CashDue => CashTenderAmount < CashTotalRounded ? CashTotalRounded - CashTenderAmount : 0m;

        public RelayCommand ShowCashTenderCommand { get; private set; }
        public RelayCommand ConfirmCashPaymentCommand { get; private set; }

        private void UpdateCashTenderCalculations()
        {
            if (!decimal.TryParse(CashTenderText, out var amount)) amount = 0m;
            CashTenderAmount = amount;
        }

        private static decimal RoundToNearestFiveCents(decimal amount) => Math.Round(amount * 20m, MidpointRounding.AwayFromZero) / 20m;
        public decimal CashTotalRounded => RoundToNearestFiveCents(Total);

        private void ConfirmCashPayment()
        {
            // If tender is enough, complete and save receipt; otherwise keep popup open
            var dueTotal = CashTotalRounded;
            if (CashTenderAmount >= dueTotal)
            {
                var number = GetNextReceiptNumber();
                SaveReceipt(number, "Cash");
                StatusMessage = $"Payment completed. Change: {CashChange:C}. Receipt #{number} saved";
                Checkout();
                IsCashTenderVisible = false;
                IsPayPopupOpen = false;
            }
            else
            {
                StatusMessage = $"Cash due: {CashDue:C}";
            }
        }

        private bool _isAdminMode;
        public bool IsAdminMode
        {
            get => _isAdminMode;
            set { if (_isAdminMode == value) return; _isAdminMode = value; OnPropertyChanged(nameof(IsAdminMode)); CommandManagerInvalidateRequerySuggested(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
