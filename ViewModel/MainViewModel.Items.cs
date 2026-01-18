using cashregister.Model;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using cashregister.Common;

namespace cashregister.ViewModel
{
    public partial class MainViewModel
    {
        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var ni in e.NewItems.OfType<Item>())
                {
                    ni.PropertyChanged += Item_PropertyChanged;
                    // Ensure it is saved if new (Id 0)
                    if (ni.Id == 0) _receiptService.SaveInventoryItem(ni);

                    if (!Categories.Contains(ni.Category))
                        Categories.Add(ni.Category);
                }
            }

            if (e.OldItems != null)
            {
                foreach (var oi in e.OldItems.OfType<Item>())
                {
                    oi.PropertyChanged -= Item_PropertyChanged;
                    _receiptService.DeleteInventoryItem(oi);

                    if (oi.Category != "base" && !Items.Any(it => it.Category == oi.Category))
                    {
                        if (Categories.Contains(oi.Category))
                            Categories.Remove(oi.Category);
                    }
                }
            }

            RebuildButtonsForSelectedCategory();
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is Item item)
            {
                _receiptService.SaveInventoryItem(item);
                // Refresh buttons if category or name changed
                if (e.PropertyName == nameof(Item.Category) || e.PropertyName == nameof(Item.Name) || e.PropertyName == nameof(Item.Price))
                {
                    RebuildButtonsForSelectedCategory();
                }
                // Refresh categories list if category changed
                if (e.PropertyName == nameof(Item.Category))
                {
                   if (!Categories.Contains(item.Category)) Categories.Add(item.Category);
                   // cleanup old empty categories not trivial without full scan, skipping for now
                }
            }
        }

        private void RebuildButtonsForSelectedCategory()
        {
            Buttons.Clear();
            foreach (var item in Items.Where(i => i.Category == SelectedCategory))
            {
                Buttons.Add(CreateButtonForItem(item));
            }
        }

        private CommandButton CreateButtonForItem(Item item)
        {
            return new CommandButton { Label = $"Add {item.Name} - {item.Price:C}", Command = new RelayCommand(_ => AddToCart(item)) };
        }

        private void AddNewItem()
        {
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

            var category = string.IsNullOrWhiteSpace(NewItemCategory) ? "base" : NewItemCategory.Trim();

            var newItem = new Item { Name = NewItemName.Trim(), Price = price, Category = category };
            Items.Add(newItem); // CollectionChanged will handle saving
            StatusMessage = $"{newItem.Name} added to {category} successfully";

            NewItemName = string.Empty;
            NewItemPriceText = string.Empty;
            NewItemCategory = string.Empty;

            SelectedCategory = category;
        }
    }
}
