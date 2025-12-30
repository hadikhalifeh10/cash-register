using System.Windows; // WPF window and application types
using cashregister.ViewModel; // reference to the viewmodel namespace

namespace cashregister // application namespace
{ // start namespace
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window // code-behind for the main window
    { // start class
        public MainWindow() // constructor
        { // start ctor
            InitializeComponent();// initialize components defined in XAML
            // DataContext is set in XAML; alternatively could set here:
            // DataContext = new MainViewModel();
        } // end ctor
    } // end class
} // end namespace