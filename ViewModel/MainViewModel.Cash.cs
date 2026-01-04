namespace cashregister.ViewModel
{
    public partial class MainViewModel
    {
        private void InitializeCashTendering()
        {
            ShowCashTenderCommand = new RelayCommand(_ => { IsCashTenderVisible = true; CashTenderText = string.Empty; });
            ConfirmCashPaymentCommand = new RelayCommand(_ => ConfirmCashPayment(), _ => true);
        }
    }
}

namespace cashregister.ViewModel
{
    public partial class MainViewModel
    {
        // Keyboard state
        private bool _isAlphaKeyboardVisible;
        public bool IsAlphaKeyboardVisible
        {
            get => _isAlphaKeyboardVisible;
            set { if (_isAlphaKeyboardVisible == value) return; _isAlphaKeyboardVisible = value; OnPropertyChanged(nameof(IsAlphaKeyboardVisible)); }
        }

        private bool _isNumericKeyboardVisible;
        public bool IsNumericKeyboardVisible
        {
            get => _isNumericKeyboardVisible;
            set { if (_isNumericKeyboardVisible == value) return; _isNumericKeyboardVisible = value; OnPropertyChanged(nameof(IsNumericKeyboardVisible)); }
        }

        private readonly KeyboardService _keyboard = new();
        public string KeyboardTarget
        {
            get => _keyboard.KeyboardTarget;
            set { if (_keyboard.KeyboardTarget == value) return; _keyboard.KeyboardTarget = value; OnPropertyChanged(nameof(KeyboardTarget)); }
        }

        public RelayCommand InsertKeyboardCharCommand { get; private set; }
        public RelayCommand KeyboardBackspaceCommand { get; private set; }
        public RelayCommand KeyboardClearCommand { get; private set; }

        private void InitializeKeyboard()
        {
            InsertKeyboardCharCommand = new RelayCommand(p => _keyboard.InsertChar(this, p as string ?? ""));
            KeyboardBackspaceCommand = new RelayCommand(_ => _keyboard.Backspace(this));
            KeyboardClearCommand = new RelayCommand(_ => _keyboard.Clear(this));
        }
    }
}
