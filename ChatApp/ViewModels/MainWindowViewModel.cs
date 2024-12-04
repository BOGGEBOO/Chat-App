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
using System.Net.Sockets;

namespace ChatApp.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public static MainWindowViewModel Instance;
        public NetworkManager NetworkManager { get; set; }
        private ICommand startServer;
        private ICommand startClient;
       // private ICommand stopServer;
        private MainWindow mainWindow;

        // Konstruktor
        public MainWindowViewModel(NetworkManager networkManager, MainWindow mainWindow)
        {
            NetworkManager = networkManager;
            networkManager.PropertyChanged += myModel_PropertyChanged;
            Instance = this;
            this.mainWindow = mainWindow;

            NetworkManager.showAcceptMessageBox += showAcceptMessageBoxEventHandler;
        }

        private string userName = "Kent";
        public string UserName
        {
            get => userName;
            set
            {
                NetworkManager.Username = value;
                userName = value;
            }
        }

        private string portNumber = "8000";

        public string PortNumber
        {
            get => portNumber;
            set
            {
                portNumber =value;
            }
        }

        private string ipAddress = "127.0.0.1";
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

        public void StopListening(Views.ConnectionRequest connectionRequest)
        {
            NetworkManager.StopListening(connectionRequest);
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

       /* public ICommand StopServer
        {
            get
            {
                if (startServer == null)
                {
                    startServer = new StopServerCommand(this);
                }
                return startServer;
            }
            set
            { startServer = value; }
        } */

        public ICommand StartClient
        {
            get {
                if( startClient == null )
                {
                    NetworkManager.Username = userName;
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
            NetworkManager.Username = userName;
            return NetworkManager.startServer(ipAddress, Int32.Parse(portNumber));
        }

        private string startClientConnection()
        {
            NetworkManager.Username = userName;
            return NetworkManager.startClient(ipAddress, Int32.Parse(portNumber));
        }


        public void startClientBoard()
        {
           /* string result = startClientConnection();
            if (result == "200 OK")
            { */
                validationMessage = "";
                GameBoard board = new GameBoard();
                
                board.ShowDialog();
            /*}
            else
            {
                
                // MessageBox.Show("Cannot start client connection!");
                ValidationMessage = result; 
            }*/
        }

        public void startGameBoard()
        {
        
            string result = startServerConnection();
            if (result  == "200 OK")
            {

                validationMessage = "";
                
                ChatWindow chat = new ChatWindow(UserName, mainWindow, NetworkManager);
                
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

        public bool AddContact(string name, TcpClient client)
        {
            return true;
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

    

        public void showAcceptMessageBoxEventHandler(object sender, ShowAcceptMessageBoxEventArgs args)
        {
            if(MessageBox.Show(args.text, "Connect with this user? ", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                NetworkManager.connectionAccepted = true;
            }
            else
            {
                NetworkManager.connectionAccepted = false;
            }
        }
    }
}

