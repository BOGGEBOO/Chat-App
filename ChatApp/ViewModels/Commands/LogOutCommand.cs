using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatApp.ViewModels;

namespace ChatApp.ViewModels.Commands
{
    internal class LogOutCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private tempViewModel parent = null;
        public LogOutCommand(tempViewModel parent)
        {
            this.parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            parent.NetworkManager.Disconnect();
            parent.ActiveConnection = false;
            //parent.Logout();
        }

    }
}
