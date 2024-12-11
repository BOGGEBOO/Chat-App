using ChatApp.Models;
using ChatApp.ViewModels.Commands;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {

        public MainViewModel(NetworkManager NetworkManager)
        {
            this.NetworkManager = NetworkManager;
            NetworkManager.PropertyChanged += NetworkManager_PropertyChanged;
            this.conversation = new ObservableCollection<Message>();
        }

        public NetworkManager NetworkManager;

        private ObservableCollection<Message> conversation;
        public ObservableCollection<Message> Conversation
        {
            get { return conversation; }
            set { conversation = value; }
        }

        private string userName = "";
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

        private string validationMessage = "";
        public string ValidationMessage
        {
            get => validationMessage;
            set
            {
                validationMessage = value;
                OnPropertyChanged();

            }
        }

        private string newMessage = "";
        public string NewMessage
        {
            get { return newMessage; }
            set
            {
                newMessage = value;
                OnPropertyChanged(newMessage);
            }
        }

        private bool activeConnection = false;
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

        private ICommand disconnect;
        public ICommand Disconnect
        {
            get
            {
                if (disconnect == null)
                {

                    disconnect = new DisconnectCommand(this);
                }
                return disconnect;
            }
            set
            {
                disconnect = value;
            }
        }

        public void Disconnecting()
        {
            NetworkManager.Disconnect();
            ActiveConnection = false;
            NetworkManager.Connected = false;
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

        public void MessageSend()
        {
            Message message = new Message("chat_message", UserName, DateTime.Now.ToString(), NewMessage);
            NetworkManager.sendMessage(message);
            Conversation.Add(message);
        }

        public void ServerStart()
        {
            if (ValidateFields(out string errorMessage))
            {
                string isSuccessful = NetworkManager.startServer(IpAddress, Int32.Parse(PortNumber));
                if (isSuccessful == "200 OK")
                {
                    ValidationMessage = "";
                }
                else
                {
                    ValidationMessage += isSuccessful;
                }
            }
            else
            {
                ValidationMessage += errorMessage;
            }
        }
        public void ClientStart()
        {
            if (ValidateFields(out string errorMessage))
            {
                string isSuccessfull = NetworkManager.startClient(IpAddress, Int32.Parse(PortNumber));
                if (isSuccessfull == "200 OK")
                {
                    ValidationMessage = "";
                }
                else
                {
                    ValidationMessage = isSuccessfull;
                }
            }
            else
            {
                ValidationMessage = errorMessage;
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
            if (e.PropertyName.Equals("connection_request"))
            {
                handleConnectionRequest();
            }
            else if (e.PropertyName.Equals("connection_response"))
            {
                handleConnectionResponse();
            }
            else if (e.PropertyName.Equals("chat_message"))
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
                if (!NetworkManager.Connected)
                {
                    return;
                }
                MessageBox.Show(message.Sender + " has disconnected due to suffering from unnatural deadness.", "whoop", MessageBoxButton.OK);
            }
        }

        private void handleConnectionRequest()
        {
            Message message = NetworkManager.LastMessage;
            Message response = new Message("connection_response", userName, DateTime.Now.ToString());
            if (MessageBox.Show("Chat with " + message.Sender + "?", "Connect with this user? ", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ActiveConnection = true;
                NetworkManager.ConnectionAccepted = true;
                response.Content = "ACCEPTED";
                NetworkManager.Sender = message.Sender;
            }
            else
            {
                NetworkManager.ConnectionAccepted = false;
                NetworkManager.Disconnect();
                response.Content = "DENIED";
            }
            NetworkManager.sendMessage(response);
        }

        private void handleConnectionResponse()
        {
            Message message = NetworkManager.LastMessage;
            if (message.Content == "ACCEPTED")
            {
                NetworkManager.Sender = message.Sender;
                ValidationMessage = "";
                ActiveConnection = true;
            }
            else if (message.Content == "DENIED")
            {
                ValidationMessage = "Server denied connection";
                NetworkManager.Close();
                ActiveConnection = false;
            }
        }

    }
}
