using System.Runtime.CompilerServices;
using System.Windows;

using ChatApp.Models;
using ChatApp.ViewModels;

namespace ChatApp.Views
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
       
        public ChatWindow(string userName, MainWindow mainWindow, NetworkManager networkManager)
        {
            InitializeComponent();
            
            mainWindow.Close();

            this.DataContext = new ChatWindowViewModel(this, userName, networkManager);
        }

        

    }
}
