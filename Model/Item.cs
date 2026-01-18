using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace cashregister.Model // namespace for model classes
{ // start namespace
    public class Item : INotifyPropertyChanged // simple model representing a product available for purchase
    { // start class
        public int Id { get; set; } // Identifier for the item, used for database persistence

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged();
            }
        } // name of the item

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set
            {
                if (_price == value) return;
                _price = value;
                OnPropertyChanged();
            }
        } // price of the item

        private string _category = "base";
        public string Category
        {
            get => _category;
            set
            {
                if (_category == value) return;
                _category = value;
                OnPropertyChanged();
            }
        } // category of the item (default to "base")

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    } // end class
} // end namespace
