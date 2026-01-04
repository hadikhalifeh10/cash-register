namespace cashregister.Model // namespace for model classes
{ // start namespace
    public class Item // simple model representing a product available for purchase
    { // start class
        public string Name { get; set; } // name of the item
        public decimal Price { get; set; } // price of the item
        public string Category { get; set; } = "base"; // category of the item (default to "base")
    } // end class
} // end namespace
