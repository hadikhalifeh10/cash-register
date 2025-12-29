using System.Windows;
using cashregister.ViewModel;

namespace cashregister
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // DataContext is set in XAML; alternatively could set here:
            // DataContext = new MainViewModel();
        }
    }
}