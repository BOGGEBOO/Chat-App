using System;
using System.Windows.Input;

namespace ChatApp.ViewModels.Commands
{
    internal class StartServerCommand : ICommand
    {

        public event EventHandler CanExecuteChanged;
        private MainViewModel parent = null;
        public StartServerCommand(MainViewModel parent) 
        {
            this.parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            parent.ServerStart();
        }
    }
}
