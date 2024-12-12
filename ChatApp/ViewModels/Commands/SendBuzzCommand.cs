using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModels.Commands
{
    class SendBuzzCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private MainViewModel parent = null;
        public SendBuzzCommand(MainViewModel parent)
        {
            this.parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            parent.BuzzSend();
        }
    }
}
