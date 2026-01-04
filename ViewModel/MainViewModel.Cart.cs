using cashregister.Model;
using System.Linq;

namespace cashregister.ViewModel
{
    public partial class MainViewModel
    {
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
            }
            else
            {
                var cartItem = new CartItem { Name = item.Name, Price = item.Price, Quantity = 1 };
                Cart.Add(cartItem);
            }

            StatusMessage = $"{item.Name} added successfully";
        }

        private void RemoveFromCart(CartItem? cartItem)
        {
            if (cartItem == null) return;
            cartItem.Quantity--;
            if (cartItem.Quantity <= 0)
            {
                cartItem.PropertyChanged -= CartItem_PropertyChanged;
                Cart.Remove(cartItem);
            }

            StatusMessage = cartItem != null ? $"{cartItem.Name} removed" : StatusMessage;
        }
    }
}
