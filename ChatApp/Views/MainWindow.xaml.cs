using System.Windows;
using ChatApp.Models;
using ChatApp.ViewModels;

namespace ChatApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel(new NetworkManager(), new HistoryManager());
        }
    }
}
