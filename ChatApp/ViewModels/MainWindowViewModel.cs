using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ChatApp.Models;
using ChatApp.Views;
using ChatApp.ViewModels.Commands;

namespace ChatApp.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {

        public NetworkManager NetworkManager { get; set; }
        private ICommand startServer;
        private ICommand startClient;

        // Konstruktor
        public MainWindowViewModel(NetworkManager networkManager)
        {
            NetworkManager = networkManager;
            networkManager.PropertyChanged += myModel_PropertyChanged;
        }

        private string userName;
        public string UserName
        {
            get => userName;
            set
            {
                userName = value;
            }
        }

        private string portNumber;

        public string PortNumber
        {
            get => portNumber;
            set
            {
                portNumber =value;
            }
        }

        private string ipAddress;
        public string IpAddress
        {
            get => ipAddress;
            set { ipAddress = value; }
        }

        private string validationMessage;
        public string ValidationMessage
        {
            get => validationMessage;
            set
            {
                validationMessage = value;
                OnPropertyChanged();

            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        

        public ICommand StartServer
        { 
            get {
                if( startServer == null )
                {
                    startServer = new StartServerCommand(this);
                }
                return startServer; 
            } 
            set 
            { startServer = value; } 
        }

        public ICommand StartClient
        {
            get {
                if( startClient == null )
                {
                    startClient = new StartClientCommand(this);
                }
                return startClient; 
            }
            set { startClient = value; }
        }

         private void myModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Message")
            {
                
            }
        } 


        private string startServerConnection()
        {
            // bool valid = ValidateFields(out "");
            return NetworkManager.startServer(ipAddress, Int32.Parse(portNumber));
        }

        private string startClientConnection()
        {
            return NetworkManager.startClient(ipAddress, Int32.Parse(portNumber));
        }


        public void startClientBoard()
        {
            string result = startClientConnection();
            if (result == "200 OK")
            {
                validationMessage = "";
                GameBoard board = new GameBoard();
                board.DataContext = this;
                board.ShowDialog();
            }
            else
            {
                
                // MessageBox.Show("Cannot start client connection!");
                ValidationMessage = result; 
            }
        }

        public void startGameBoard()
        {
        
            string result = startServerConnection();
            if (result  == "200 OK")
            {
                validationMessage = "";
                ChatWindow chat = new ChatWindow(userName);
                chat.DataContext = this;
                chat.ShowDialog();
                /* 
                 * GameBoard board = new GameBoard();
                 * board.DataContext = this;
                 * board.ShowDialog(); 
                */
            }
            else
            {
                ValidationMessage = result;
                // MessageBox.Show("Cannot start server connection!");
            }



        }

        public bool ValidateFields(out string errorMessage)
        {
            errorMessage = "";
            if (string.IsNullOrWhiteSpace(UserName))
            {
                errorMessage += "Username is required.\n";
            }

            if (string.IsNullOrWhiteSpace(IpAddress))
            {
                errorMessage += "Valid IP address is required.\n";
            }

            if (string.IsNullOrWhiteSpace(portNumber) || !int.TryParse(portNumber, out int _))
            {
                errorMessage += "Valid port number is required.\n";
            }

            return string.IsNullOrEmpty(errorMessage);
        }   

    }
}

