using System;
using System.Windows.Input;

namespace ChatApp.ViewModels.Commands
{
    internal class StartClientCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private MainViewModel parent = null;
        public StartClientCommand(MainViewModel parent)
        {
            this.parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
           parent.ClientStart();
        }
    }
}
