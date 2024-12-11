using ChatApp.Models;
using ChatApp.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace ChatApp.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {

        public MainViewModel(NetworkManager networkManager, HistoryManager historyManager)
        {
            this.networkManager = networkManager;
            networkManager.PropertyChanged += networkManager_PropertyChanged;
            networkManager.PropertyChanged += networkManager_StatusPropertyChanged;
            this.historyManager = historyManager;
            this.conversation = new ObservableCollection<Message>();
        }

        private NetworkManager networkManager;

        private HistoryManager historyManager;

        private ObservableCollection<Message> conversation;
        public ObservableCollection<Message> Conversation
        {
            get { return conversation; }
            set { conversation = value; }
        }

        private List<Contact> contacts;

        private ObservableCollection<Contact> shownContacts;
        public ObservableCollection<Contact> ShownContacts
        {
            get { return shownContacts; }
            set { shownContacts = value; }

        }



        private string userName = "";
        public string UserName
        {
            get => userName;
            set
            {
                networkManager.Username = value;
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

        private string searchString;
        public string SearchString
        {
            get => searchString;
            set => searchString = value;
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
                    networkManager.Username = UserName;
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
                    networkManager.Username = UserName;
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
            contacts.Add(new Contact(networkManager.Sender, DateTime.Now.ToString(), new List<Message>(Conversation)));
            ShownContacts = new ObservableCollection<Contact>(contacts);
            WriteHistory();
            networkManager.Disconnect();
            ActiveConnection = false;
            networkManager.Connected = false;
            Conversation.Clear();
        }

        private ICommand sendMessage;
        public ICommand SendMessage
        {
            get
            {
                if (sendMessage == null)
                {
                    Message message = new Message("chat_message", userName, DateTime.Now.ToString("g"), NewMessage);
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
            Message message = new Message("chat_message", UserName, DateTime.Now.ToString("g"), NewMessage);
            networkManager.sendMessage(message);
            Conversation.Add(message);
        }

        private ICommand search;
        public ICommand Search
        {
            get 
            { 
                if(search == null)
                {
                    search = new SearchCommand(this);
                }
                return search; 
            }
            set 
            {
                search = value; 
            }
        }

        public void SearchContacts()
        {
            List<Contact> holder = historyManager.Search(searchString, contacts);
            ShownContacts = new ObservableCollection<Contact>(holder);
            OnPropertyChanged(nameof(ShownContacts));
        }

        public void ServerStart()
        {
            if (ValidateFields(out string errorMessage))
            {
                networkManager.startServer(IpAddress, Int32.Parse(PortNumber));
                contacts = historyManager.Read(userName);
                ShownContacts = new ObservableCollection<Contact>(contacts);
                OnPropertyChanged(nameof(ShownContacts));
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
                networkManager.startClient(IpAddress, Int32.Parse(PortNumber));
                contacts = historyManager.Read(userName);
                ShownContacts = new ObservableCollection<Contact>(contacts);
                OnPropertyChanged(nameof(ShownContacts));
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

        private void networkManager_StatusPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("no_listener"))
            {
                ValidationMessage = "No one is listening to this port/IP adress";
            }
            else if(e.PropertyName.Equals("busy_adress"))
            {
                ValidationMessage = "This adress is already in use";
            }
            else if(e.PropertyName.Equals("no_start"))
            {
                ValidationMessage = "Error starting server. Please check your information or try again later.";
            }
            else if(e.PropertyName.Equals("connection_error"))
            {
                ValidationMessage = "Error when connecting. Check your information and try again.";
            }
        }

        private void networkManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
                Conversation.Add(networkManager.LastMessage);
            }
            else if (e.PropertyName.Equals("disconnect"))
            {
                ActiveConnection = false;
                WriteHistory();
                Message message = networkManager.LastMessage;
                MessageBox.Show(message.Sender + " has disconnected due to suffering from dead.", "whoop", MessageBoxButton.OK);
            }
            else if (e.PropertyName.Equals("dramatic_disconnect"))
            {
                ActiveConnection = false;
                Message message = networkManager.LastMessage;
                if (!networkManager.Connected)
                {
                    return;
                }
                MessageBox.Show(message.Sender + " has disconnected due to suffering from unnatural deadness.", "whoop", MessageBoxButton.OK);
                WriteHistory();
            }
        }

        private void handleConnectionRequest()
        {
            Message message = networkManager.LastMessage;
            Message response = new Message("connection_response", userName, DateTime.Now.ToString("g"));
            if (MessageBox.Show("Chat with " + message.Sender + "?", "Connect with this user? ", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ActiveConnection = true;
                networkManager.ConnectionAccepted = true;
                response.Content = "ACCEPTED";
                networkManager.sendMessage(response);
                networkManager.Sender = message.Sender;
                Message connectionMessage = new Message("chat_message", "System", DateTime.Now.ToString("g"), "You are now chatting with " + message.Sender);
                Conversation.Add(connectionMessage);
            }
            else
            {
                networkManager.ConnectionAccepted = false;
                response.Content = "DENIED";
                networkManager.sendMessage(response);
                networkManager.Close();
            }
        }

        private void handleConnectionResponse()
        {
            Message message = networkManager.LastMessage;
            if (message.Content == "ACCEPTED")
            {
                networkManager.Sender = message.Sender;
                ValidationMessage = "";
                ActiveConnection = true;
                Message connectionMessage = new Message("chat_message", "System", DateTime.Now.ToString("g"), "You are now chatting with " + message.Sender);
                Conversation.Add(connectionMessage);
            }
            else if (message.Content == "DENIED")
            {
                ValidationMessage = "Server denied connection";
                networkManager.Close();
                ActiveConnection = false;
            }
        }

        private void WriteHistory()
        {
            historyManager.Write(contacts, userName);
        }

        void OnProcessExit(object sender, EventArgs e)
        {
            WriteHistory();
        }


    }
}

