using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using ChatApp.Models;
using ChatApp.ViewModels;

namespace ChatApp.Views
{
    /// <summary>
    /// Interaction logic for ConnectionRequest.xaml
    /// </summary>
    public partial class ConnectionRequest : Window
    {
        public ConnectionRequest()
        {
            InitializeComponent();
        }

        private void StopSearching(object sender, RoutedEventArgs e)
        {
            this.Close();
        } 

        

        public bool ShowMessageBox(string name)
        {
            string messageText = $"Do you want to accept connection from {name}?";
            string caption = "Friend request";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage image = MessageBoxImage.Question;
            MessageBoxResult result;
            result = MessageBox.Show(messageText, caption, button, image, MessageBoxResult.Yes);

            return result == MessageBoxResult.Yes;
        }
    }
}
