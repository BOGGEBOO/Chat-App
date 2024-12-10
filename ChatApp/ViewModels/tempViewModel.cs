using ChatApp.Models;
using ChatApp.ViewModels.Commands;
using ChatApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModels
{
    internal class tempViewModel : BaseViewModel
    {

        public tempViewModel(NetworkManager NetworkManager) 
        {
            this.NetworkManager = NetworkManager;
            NetworkManager.PropertyChanged += NetworkManager_PropertyChanged;
            NetworkManager.showAcceptMessageBox += showAcceptMessageBoxEventHandler;
            this.conversation = new ObservableCollection<Message>();
        }

        public NetworkManager NetworkManager;
        
        private tempWindow window;

        private ObservableCollection<Message> conversation;
        public ObservableCollection<Message> Conversation
        {
            get { return conversation; }
            set { conversation = value; }
        }

        private string userName;
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
                portNumber = value;
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

        private string newMessage;
        public string NewMessage
        {
            get { return newMessage; }
            set
            {
                newMessage = value;
                OnPropertyChanged(newMessage);
            }
        }

        private bool activeConnection;
        public bool ActiveConnection
        {
            get { return activeConnection; }
            set 
            { 
                activeConnection = value; 
                OnPropertyChanged(nameof(ActiveConnection));
            }
        }

        private ICommand startServer;
        public ICommand StartServer
        {
            get
            {
                if (startServer == null)
                {
                    NetworkManager.Username = UserName;
                    startServer = new StartServerCommand(this);
                }
                return startServer;
            }
            set
            { startServer = value; }
        }

        private ICommand startClient;
        public ICommand StartClient
        {
            get
            {
                if (startClient == null)
                {
                    NetworkManager.Username = UserName;
                    startClient = new StartClientCommand(this);
                }
                return startClient;
            }
            set { startClient = value; }
        }

        private ICommand disconnect; //Döpt om, måste ändra Command
        public ICommand Disconnect
        {
            get
            {
                if (disconnect == null)
                {
                    disconnect = new LogOutCommand(this);
                }
                return disconnect;
            }
            set
            {
                disconnect = value;
            }
        }

        private ICommand sendMessage;
        public ICommand SendMessage
        {
            get
            {
                if (sendMessage == null)
                {
                    Message message = new Message("chat_message", userName, DateTime.Now.ToString(), NewMessage);
                    sendMessage = new SendMessageCommand(this);
                }
                return sendMessage;
            }
            set
            {
                sendMessage = value;
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

        private void NetworkManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("chat_message"))
            {
                Conversation.Add(NetworkManager.LastMessage);
            }
            else if (e.PropertyName.Equals("disconnect"))
            {
                ActiveConnection = false;
                Message message = NetworkManager.LastMessage;
                MessageBox.Show(message.Sender + " has disconnected due to suffering from dead.", "whoop", MessageBoxButton.OK);
            }
            else if (e.PropertyName.Equals("dramatic_disconnect"))
            {
                ActiveConnection = false;
                Message message = NetworkManager.LastMessage;
                if(message.Sender == userName)
                {
                    return;
                }
                MessageBox.Show(message.Sender + " has disconnected due to suffering from unnatural deadness.", "whoop", MessageBoxButton.OK);
            }
        }

        public void showAcceptMessageBoxEventHandler(object sender, ShowAcceptMessageBoxEventArgs args)
        {
            if (MessageBox.Show(args.text, "Connect with this user? ", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ActiveConnection = true;
                NetworkManager.connectionAccepted = true;
            }
            else
            {
                NetworkManager.connectionAccepted = false;
            }
        }
    }
}
