using cashregister.Model;
using System.Linq;

namespace cashregister.ViewModel
{
    public partial class MainViewModel
    {
        private void PayAndCloseWithReceipt(string method)
        {
            if (!Cart.Any())
            {
                StatusMessage = "Cart is empty";
                IsPayPopupOpen = false;
                return;
            }

            if (method == "Cash")
            {
                // show cash tender UI instead of finishing immediately
                IsCashTenderVisible = true;
                return;
            }

            var number = GetNextReceiptNumber();
            SaveReceipt(number, method);
            StatusMessage = $"Payment completed. Receipt #{number} saved";
            Checkout();
            IsPayPopupOpen = false;
        }

        public decimal Total
        {
            get
            {
                var raw = Subtotal + Gst + Qst;
                var discounted = raw * (1 - TotalDiscountPercent);
                return discounted;
            }
        }

        // initialize cash commands in ctor via a helper in a separate partial
        private void InitializePaymentsUi()
        {
            // called from ctor
            InitializeSplit();
        }
    }
}
