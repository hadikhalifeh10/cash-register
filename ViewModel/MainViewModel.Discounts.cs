using cashregister.Model;

namespace cashregister.ViewModel
{
    public partial class MainViewModel
    {
        private void ApplyItemDiscount(cashregister.Model.CartItem? item)
        {
            if (item == null)
            {
                StatusMessage = "Select a cart item to discount";
                return;
            }

            if (!TryParsePercent(DiscountText, out var percent))
            {
                StatusMessage = "Enter discount like 10 or 10%";
                return;
            }

            item.DiscountPercent = percent;
            StatusMessage = $"Applied {percent:P0} discount to {item.Name}";
            RaiseTotalsChanged();
        }

        private void ApplyTotalDiscount()
        {
            if (!TryParsePercent(DiscountText, out var percent))
            {
                StatusMessage = "Enter discount like 10 or 10%";
                return;
            }

            TotalDiscountPercent = percent;
            StatusMessage = $"Applied {percent:P0} discount to total";
        }

        private static bool TryParsePercent(string? text, out decimal value)
        {
            value = 0m;
            if (string.IsNullOrWhiteSpace(text)) return false;
            text = text.Trim();
            if (text.EndsWith("%")) text = text.Substring(0, text.Length - 1);
            if (!decimal.TryParse(text, out var num)) return false;
            value = num / 100m;
            return true;
        }
    }
}
