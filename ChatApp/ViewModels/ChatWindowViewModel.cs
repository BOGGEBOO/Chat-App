using ChatApp.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatApp.Models;
using ChatApp.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChatApp.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;

namespace ChatApp.ViewModels
{
    public class ChatWindowViewModel : BaseViewModel
    {
       /* public static ChatWindowViewModel Instance;



        private ICommand logOut;
        public NetworkManager NetworkManager { get; set; }

        private ChatWindow chatWindow;

        private ObservableCollection<Message> conversation;
        public ObservableCollection<Message> Conversation
        {
            get { return conversation; }
            set { conversation = value; }
        }


        public ChatWindowViewModel(ChatWindow chatWindow, string uName, NetworkManager networkManager) 
        {
            System.Diagnostics.Debug.WriteLine(uName);
            this.userName = uName;
            this.NetworkManager = networkManager;
            this.chatWindow = chatWindow;
            this.chatPartner = networkManager.Sender;
            Instance = this;
            
            conversation = new ObservableCollection<Message>();
            NetworkManager.PropertyChanged += NetworkManager_PropertyChanged;
            
        }

        private string userName;
        public string Username
        {
            get => this.userName;
            set
            {
                this.userName = value;
            }
        }

        private string chatPartner;
        public string ChatPartner
        {
            get { return chatPartner; }
            set 
            { 
                chatPartner = value;
                OnPropertyChanged(chatPartner);
            }
        }

        private void NetworkManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("chat_message"))
            {
                Conversation.Add(NetworkManager.LastMessage);
            }
            else if (e.PropertyName.Equals("disconnect"))
            {
                Message message = NetworkManager.LastMessage;
                MessageBox.Show(message.Sender + " has disconnected due to suffering from dead.", "whoop", MessageBoxButton.OK);
            }
            else if(e.PropertyName.Equals("dramatic_disconnect"))
            {
                Message message = NetworkManager.LastMessage;
                MessageBox.Show(message.Sender + " has disconnected due to suffering from unnatural deadness.", "whoop", MessageBoxButton.OK);
            }
        }

        private NetworkManager chat;
        public NetworkManager Chat
        {
            get { return chat; }
            set { chat = value; }
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
        

        private ICommand sendMessage;
        public ICommand SendMessage
        {
            get
            {
                if(sendMessage == null)
                {
                    Message message = new Message("chat_message", userName, DateTime.Now.ToString(), NewMessage);
                    //Conversation.Add(message);
                    sendMessage = new SendMessageCommand(this);
                    
                    //Scrollan.ScrollToBottom(); 
                }
                return sendMessage;
            }
            set 
            { 
                sendMessage = value; 
            }
        }


        public ICommand LogOut
        {
            get
            {
                if(logOut == null)
                {
                    logOut = new LogOutCommand(this);
                }
                return logOut;
            }
            set
            {
                logOut = value;
            }
        }

        public void Logout()
        {
            MainWindow main = new MainWindow(chatWindow);
            main.ShowDialog();
        }


        */
    }
}
