using System;
using System.Windows.Input;

namespace ChatApp.ViewModels.Commands
{
    internal class DisconnectCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private MainViewModel parent = null;
        public DisconnectCommand(MainViewModel parent)
        {
            this.parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            parent.Disconnecting();
        }

    }
}
