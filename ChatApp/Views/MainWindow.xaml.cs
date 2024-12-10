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
            
           // this.DataContext = new MainWindowViewModel(new NetworkManager(), this);
        }
        public MainWindow(ChatWindow chat)
        {
            InitializeComponent();
            chat.Close();
           // this.DataContext = new MainWindowViewModel(new NetworkManager(), this);
        }
    }
}
