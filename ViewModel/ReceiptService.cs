using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using cashregister.Model;

namespace cashregister.ViewModel
{
    // Encapsulates receipt storage, numbering, saving, and loading
    public class ReceiptService
    {
        public ReceiptService()
        {
            EnsureReceiptsFolder();
        }

        public void EnsureReceiptsFolder()
        {
            var dir = GetReceiptsFolder();
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public string GetReceiptsFolder()
        {
            // Place Receipts at the project root alongside Model and ViewModel
            // During debugging, BaseDirectory points to bin/Debug/... so go up three levels
            var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
            return Path.Combine(projectRoot, "Receipts");
        }

        public int GetNextReceiptNumber()
        {
            EnsureReceiptsFolder();
            var dir = GetReceiptsFolder();
            var files = Directory.GetFiles(dir, "*.txt");
            var max = 0;
            foreach (var f in files)
            {
                var name = Path.GetFileNameWithoutExtension(f);
                if (int.TryParse(name, out var n))
                {
                    if (n > max) max = n;
                }
            }
            return max + 1;
        }

        public string GetReceiptPath(int number) => Path.Combine(GetReceiptsFolder(), $"{number}.txt");

        public void SaveReceipt(int number, string paymentMethod, IEnumerable<CartItem> cart, decimal subtotal, decimal gst, decimal qst, decimal totalDiscountPercent, decimal total)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Receipt: {number}");
            sb.AppendLine($"Date: {DateTime.Now:yyyy-MM-dd HH:mm}");
            sb.AppendLine($"Payment: {paymentMethod}");
            sb.AppendLine("Items:");
            foreach (var ci in cart)
            {
                var discountText = ci.DiscountPercent > 0 ? $" (-{ci.DiscountPercent:P0})" : "";
                sb.AppendLine($"- {ci.Name} x{ci.Quantity} @ {ci.Price:C}{discountText} = {ci.Subtotal:C}");
            }
            sb.AppendLine($"Subtotal: {subtotal:C}");
            sb.AppendLine($"GST: {gst:C}");
            sb.AppendLine($"QST: {qst:C}");
            if (totalDiscountPercent > 0)
            {
                sb.AppendLine($"Total Discount: {totalDiscountPercent:P0}");
            }
            sb.AppendLine($"Total: {total:C}");

            File.WriteAllText(GetReceiptPath(number), sb.ToString());
        }

        public bool TryLoadReceipt(int number, ObservableCollection<CartItem> cart, out decimal totalDiscountPercent)
        {
            totalDiscountPercent = 0m;
            var path = GetReceiptPath(number);
            if (!File.Exists(path)) return false;

            var lines = File.ReadAllLines(path);
            cart.Clear();
            totalDiscountPercent = 0;

            foreach (var line in lines)
            {
                if (line.StartsWith("Total Discount:"))
                {
                    var percentText = line.Substring("Total Discount:".Length).Trim();
                    percentText = percentText.Trim('%');
                    if (decimal.TryParse(percentText, out var num))
                    {
                        totalDiscountPercent = num / 100m;
                    }
                }
                else if (line.StartsWith("- "))
                {
                    try
                    {
                        var s = line.Substring(2);
                        var atIdx = s.IndexOf(" @ ");
                        var left = s.Substring(0, atIdx);
                        var nameEndIdx = left.LastIndexOf(" x");
                        var name = left.Substring(0, nameEndIdx);
                        var qty = int.Parse(left.Substring(nameEndIdx + 2));

                        var right = s.Substring(atIdx + 3);
                        var eqIdx = right.LastIndexOf(" = ");
                        var pricePart = right.Substring(0, eqIdx).Trim();
                        var discountPercent = 0m;
                        var discountIdx = pricePart.IndexOf("(-");
                        if (discountIdx >= 0)
                        {
                            var priceOnly = pricePart.Substring(0, discountIdx).Trim();
                            var discountPart = pricePart.Substring(discountIdx + 2).Trim(); // like "10%)"
                            discountPart = discountPart.TrimEnd(')');
                            discountPart = discountPart.TrimEnd('%');
                            decimal.TryParse(discountPart, out discountPercent);
                            discountPercent /= 100m;
                            pricePart = priceOnly;
                        }
                        var digits = new string(pricePart.Where(ch => char.IsDigit(ch) || ch == '.' || ch == ',').ToArray());
                        if (!decimal.TryParse(digits, out var price)) price = 0m;

                        var cartItem = new CartItem { Name = name, Price = price, Quantity = qty, DiscountPercent = discountPercent };
                        cart.Add(cartItem);
                    }
                    catch
                    {
                        // ignore parse errors for robustness
                    }
                }
            }
            return true;
        }
    }
}
