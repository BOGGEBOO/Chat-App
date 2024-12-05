using ChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModels.Commands
{
    internal class SendMessageCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private ChatWindowViewModel parent = null;
        private Message message;
        public SendMessageCommand(ChatWindowViewModel parent)
        {
            this.parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            message = new Message("chat_message", parent.Username, DateTime.Now.ToString(), parent.NewMessage);
            parent.NetworkManager.sendMessage(message);
            parent.Conversation.Add(message);        
        }
    }
}
