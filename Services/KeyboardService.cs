namespace cashregister.Services
{
    // Encapsulates on-screen keyboard routing and operations
    public class KeyboardService
    {
        public string KeyboardTarget { get; set; } = string.Empty;

        public void InsertChar(cashregister.ViewModel.MainViewModel vm, string ch)
        {
            if (KeyboardTarget == nameof(vm.NewItemName)) vm.NewItemName += ch;
            else if (KeyboardTarget == nameof(vm.NewItemPriceText)) vm.NewItemPriceText += ch;
            else if (KeyboardTarget == nameof(vm.NewItemCategory)) vm.NewItemCategory += ch;
            else if (KeyboardTarget == nameof(vm.DiscountText)) vm.DiscountText += ch;
            else if (KeyboardTarget == nameof(vm.CashTenderText)) vm.CashTenderText += ch;
            else if (KeyboardTarget == nameof(vm.LoadReceiptNumber)) vm.LoadReceiptNumber += ch;
        }

        public void Backspace(cashregister.ViewModel.MainViewModel vm)
        {
            string Bs(string s) => string.IsNullOrEmpty(s) ? s : s.Substring(0, s.Length - 1);
            if (KeyboardTarget == nameof(vm.NewItemName)) vm.NewItemName = Bs(vm.NewItemName);
            else if (KeyboardTarget == nameof(vm.NewItemPriceText)) vm.NewItemPriceText = Bs(vm.NewItemPriceText);
            else if (KeyboardTarget == nameof(vm.NewItemCategory)) vm.NewItemCategory = Bs(vm.NewItemCategory);
            else if (KeyboardTarget == nameof(vm.DiscountText)) vm.DiscountText = Bs(vm.DiscountText);
            else if (KeyboardTarget == nameof(vm.CashTenderText)) vm.CashTenderText = Bs(vm.CashTenderText);
            else if (KeyboardTarget == nameof(vm.LoadReceiptNumber)) vm.LoadReceiptNumber = Bs(vm.LoadReceiptNumber);
        }

        public void Clear(cashregister.ViewModel.MainViewModel vm)
        {
            if (KeyboardTarget == nameof(vm.NewItemName)) vm.NewItemName = string.Empty;
            else if (KeyboardTarget == nameof(vm.NewItemPriceText)) vm.NewItemPriceText = string.Empty;
            else if (KeyboardTarget == nameof(vm.NewItemCategory)) vm.NewItemCategory = string.Empty;
            else if (KeyboardTarget == nameof(vm.DiscountText)) vm.DiscountText = string.Empty;
            else if (KeyboardTarget == nameof(vm.CashTenderText)) vm.CashTenderText = string.Empty;
            else if (KeyboardTarget == nameof(vm.LoadReceiptNumber)) vm.LoadReceiptNumber = string.Empty;
        }
    }
}
