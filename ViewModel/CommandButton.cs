using System.Windows.Input; // ICommand interface for command binding

namespace cashregister.ViewModel // namespace for viewmodel helper types
{ // start namespace
    public class CommandButton // simple class representing a button exposed by the viewmodel
    { // start class
        public string Label { get; set; } // text displayed on the button
        public ICommand Command { get; set; } // command that will be executed when the button is clicked
    } // end class
} // end namespace
