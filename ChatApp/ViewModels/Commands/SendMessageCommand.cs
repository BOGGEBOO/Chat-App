using ChatApp.Models;
using System;
using System.Windows.Input;

namespace ChatApp.ViewModels.Commands
{
    internal class SendMessageCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private MainViewModel parent = null;
        public SendMessageCommand(MainViewModel parent)
        {
            this.parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            parent.MessageSend();        
        }
    }
}
