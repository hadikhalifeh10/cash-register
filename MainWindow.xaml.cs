using System.Windows; // WPF window and application types
using System.Windows.Controls;
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

        private void TextBox_GotFocus_SetAlpha(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModel.MainViewModel vm)
            {
                vm.KeyboardTarget = (sender as TextBox)?.Tag as string ?? (sender as TextBox)?.Name ?? string.Empty;
                vm.IsAlphaKeyboardVisible = true;
                vm.IsNumericKeyboardVisible = false;
            }
        }

        private void TextBox_GotFocus_SetNumeric(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModel.MainViewModel vm)
            {
                vm.KeyboardTarget = (sender as TextBox)?.Tag as string ?? (sender as TextBox)?.Name ?? string.Empty;
                vm.IsAlphaKeyboardVisible = false;
                vm.IsNumericKeyboardVisible = true;
            }
        }
    } // end class
} // end namespace