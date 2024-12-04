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

namespace ChatApp.ViewModels
{
    public class ChatWindowViewModel : INotifyPropertyChanged
    {

        //private ICommand AcceptConnection;

        public static ChatWindowViewModel Instance;

        private ICommand stopServer;
        public NetworkManager NetworkManager { get; set; }

        private ChatWindow chat;
        public ChatWindowViewModel(ChatWindow chat, string uName, NetworkManager networkManager) 
        {
            System.Diagnostics.Debug.WriteLine(uName);
            this.userName = uName;
            this.NetworkManager = networkManager;
            this.chat = chat;
            Instance = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
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
        public ICommand AcceptConnection
        {
            get
            {
                /*
                if (AcceptConnection == null)
                {
                    AcceptConnection = new AcceptConnectionCommand(this);
                }
                */
                return AcceptConnection;
            }
            set
            { AcceptConnection = value; }
        }

        public ICommand StopServer
        {
            get
            {
                if(stopServer == null)
                {
                    stopServer = new StopServerCommand(this);
                }
                return stopServer;
            }
            set
            {
                stopServer = value;
            }
        }

        public void Logout()
        {
            MainWindow main = new MainWindow(chat);
            main.ShowDialog();
        }



    }
}
