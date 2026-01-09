using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using cashregister.Model;

namespace cashregister.ViewModel
{
    public partial class MainViewModel
    {
        // Split bill state
        public ObservableCollection<SplitPerson> SplitPeople { get; } = new();
        public ObservableCollection<SplitCartItem> SplitItems { get; } = new();

        private bool _isSplitPopupOpen;
        public bool IsSplitPopupOpen
        {
            get => _isSplitPopupOpen;
            set { if (_isSplitPopupOpen == value) return; _isSplitPopupOpen = value; OnPropertyChanged(nameof(IsSplitPopupOpen)); }
        }

        private bool _showPaymentMethods;
        public bool ShowPaymentMethods
        {
            get => _showPaymentMethods;
            set { if (_showPaymentMethods == value) return; _showPaymentMethods = value; OnPropertyChanged(nameof(ShowPaymentMethods)); }
        }

        private int _selectedSplitIndex;
        public int SelectedSplitIndex
        {
            get => _selectedSplitIndex;
            set
            {
                var coerced = value;
                if (SplitPeople.Count == 0) coerced = 0;
                else if (value < 0) coerced = 0;
                else if (value >= SplitPeople.Count) coerced = SplitPeople.Count - 1;
                if (_selectedSplitIndex == coerced) return;
                _selectedSplitIndex = coerced;
                OnPropertyChanged(nameof(SelectedSplitIndex));
                OnPropertyChanged(nameof(SelectedSplitPerson));
            }
        }
        public SplitPerson? SelectedSplitPerson => (SplitPeople.Count > 0 && SelectedSplitIndex >= 0 && SelectedSplitIndex < SplitPeople.Count) ? SplitPeople[SelectedSplitIndex] : null;

        public RelayCommand StartSplitBillCommand { get; private set; }
        public RelayCommand AddSplitPersonCommand { get; private set; }
        public RelayCommand RemoveSplitPersonCommand { get; private set; }
        public RelayCommand ConfirmSplitPaymentCommand { get; private set; }
        public RelayCommand CancelSplitCommand { get; private set; }
        public RelayCommand ChooseOneBillCommand { get; private set; }
        public RelayCommand ClearSplitItemAssignmentCommand { get; private set; }
        public RelayCommand PrevSplitPersonCommand { get; private set; }
        public RelayCommand NextSplitPersonCommand { get; private set; }

        private void InitializeSplit()
        {
            StartSplitBillCommand = new RelayCommand(_ => StartSplit());
            AddSplitPersonCommand = new RelayCommand(_ => AddSplitPerson());
            RemoveSplitPersonCommand = new RelayCommand(_ => RemoveSplitPerson(), _ => SplitPeople.Count > 1);
            ConfirmSplitPaymentCommand = new RelayCommand(_ => ConfirmSplitPayment(), _ => CanConfirmSplit());
            CancelSplitCommand = new RelayCommand(_ => { IsSplitPopupOpen = false; });
            ChooseOneBillCommand = new RelayCommand(_ => { ShowPaymentMethods = true; });
            ClearSplitItemAssignmentCommand = new RelayCommand(p => { if (p is SplitCartItem si) { si.AssignedTo = null; } });
            PrevSplitPersonCommand = new RelayCommand(_ => { if (SplitPeople.Count == 0) return; SelectedSplitIndex = (SelectedSplitIndex - 1 + SplitPeople.Count) % SplitPeople.Count; });
            NextSplitPersonCommand = new RelayCommand(_ => { if (SplitPeople.Count == 0) return; SelectedSplitIndex = (SelectedSplitIndex + 1) % SplitPeople.Count; });
        }

        private void StartSplit()
        {
            // Prepare split context from current cart
            SplitPeople.Clear();
            SplitItems.Clear();
            AddSplitPerson();

            foreach (var ci in Cart)
            {
                var vm = new SplitCartItem(this, ci);
                SplitItems.Add(vm);
            }

            // show split UI
            IsSplitPopupOpen = true;
            // hide payment popup if open
            IsPayPopupOpen = false;
            SelectedSplitIndex = 0;
            NotifySplitTotalsChanged();
        }

        private void AddSplitPerson()
        {
            var name = $"P{SplitPeople.Count + 1}";
            var p = new SplitPerson(this, name);
            SplitPeople.Add(p);
            SelectedSplitIndex = SplitPeople.Count - 1;
            NotifySplitTotalsChanged();
        }

        private void RemoveSplitPerson()
        {
            if (SplitPeople.Count <= 1) return;
            var idx = SelectedSplitIndex;
            var toRemove = SplitPeople[idx];
            // unassign any items assigned to this person
            foreach (var si in SplitItems.Where(x => ReferenceEquals(x.AssignedTo, toRemove)).ToList())
            {
                si.AssignedTo = null;
            }
            SplitPeople.RemoveAt(idx);
            if (SplitPeople.Count == 0) SelectedSplitIndex = 0;
            else if (idx >= SplitPeople.Count) SelectedSplitIndex = SplitPeople.Count - 1;
            else SelectedSplitIndex = idx;
            NotifySplitTotalsChanged();
        }

        // Split totals aggregation
        public decimal SplitAssignedSubtotal => SplitPeople.Sum(p => p.Subtotal);
        public decimal SplitAssignedGst => SplitPeople.Sum(p => p.Gst);
        public decimal SplitAssignedQst => SplitPeople.Sum(p => p.Qst);
        public decimal SplitAssignedTotal => SplitPeople.Sum(p => p.Total);
        public decimal SplitLeftToPay => Total - SplitAssignedTotal;
        public decimal SplitChange => SplitAssignedTotal > Total ? SplitAssignedTotal - Total : 0m;

        public void NotifySplitTotalsChanged()
        {
            // notify per-person and aggregate totals
            foreach (var p in SplitPeople)
            {
                p.NotifyTotalsChanged();
            }
            OnPropertyChanged(nameof(SelectedSplitPerson));
            OnPropertyChanged(nameof(SplitAssignedSubtotal));
            OnPropertyChanged(nameof(SplitAssignedGst));
            OnPropertyChanged(nameof(SplitAssignedQst));
            OnPropertyChanged(nameof(SplitAssignedTotal));
            OnPropertyChanged(nameof(SplitLeftToPay));
            OnPropertyChanged(nameof(SplitChange));
            // also update CanExecute of confirm and remove commands
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        private bool CanConfirmSplit()
        {
            if (!SplitPeople.Any()) return false;
            // require all assigned items to cover the total (within 1 cent tolerance)
            if (Math.Abs(SplitLeftToPay) > 0.01m) return false;
            // each person with items must have a payment method
            foreach (var p in SplitPeople)
            {
                if (p.AssignedItems.Any() && string.IsNullOrWhiteSpace(p.PaymentMethod)) return false;
            }
            return true;
        }

        private void ConfirmSplitPayment()
        {
            // Save a separate receipt per person with items
            var numbers = new System.Collections.Generic.List<int>();
            foreach (var p in SplitPeople)
            {
                var personItems = p.AssignedItems.Select(si => si.Item).ToList();
                if (personItems.Count == 0) continue;
                var number = GetNextReceiptNumber();
                _receiptService.SaveReceipt(number, p.PaymentMethod, personItems, p.Subtotal, p.Gst, p.Qst, TotalDiscountPercent, p.Total);
                numbers.Add(number);
            }

            StatusMessage = $"Split payment completed. Receipts: #{string.Join(", #", numbers)}";
            // finish transaction
            Checkout();
            IsSplitPopupOpen = false;
        }
    }

    public class SplitPerson : INotifyPropertyChanged
    {
        private readonly MainViewModel _owner;
        private string _name;
        private string _paymentMethod = string.Empty; // Credit/Debit/Cash
        public SplitPerson(MainViewModel owner, string name)
        {
            _owner = owner; _name = name;
        }
        public string Name { get => _name; set { if (_name == value) return; _name = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name))); } }
        public string PaymentMethod { get => _paymentMethod; set { if (_paymentMethod == value) return; _paymentMethod = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PaymentMethod))); } }

        // Items assigned to this person
        public System.Collections.Generic.IEnumerable<SplitCartItem> AssignedItems => _owner.SplitItems.Where(si => ReferenceEquals(si.AssignedTo, this));

        // Computed totals for this person (uses same tax/discount model)
        public decimal Subtotal => AssignedItems.Sum(si => si.Item.Subtotal);
        public decimal Gst => Math.Round(Subtotal * 0.05m, 2);
        public decimal Qst => Math.Round(Subtotal * 0.09975m, 2);
        public decimal Total
        {
            get
            {
                var raw = Subtotal + Gst + Qst;
                return Math.Round(raw * (1 - _owner.TotalDiscountPercent), 2);
            }
        }

        internal void NotifyTotalsChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Subtotal)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Gst)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Qst)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Total)));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class SplitCartItem : INotifyPropertyChanged
    {
        private readonly MainViewModel _owner;
        public CartItem Item { get; }
        private SplitPerson? _assignedTo;
        public SplitPerson? AssignedTo
        {
            get => _assignedTo;
            set
            {
                if (ReferenceEquals(_assignedTo, value)) return;
                _assignedTo = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AssignedTo)));
                _owner.NotifySplitTotalsChanged();
            }
        }
        public SplitCartItem(MainViewModel owner, CartItem item) { _owner = owner; Item = item; Item.PropertyChanged += (_, __) => _owner.NotifySplitTotalsChanged(); }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
