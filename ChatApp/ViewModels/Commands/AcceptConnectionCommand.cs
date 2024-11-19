using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModels.Commands
{
    class AcceptConnectionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private ChatWindowViewModel parent = null;
        public AcceptConnectionCommand(ChatWindowViewModel parent)
        {
            this.parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            System.Diagnostics.Debug.WriteLine("QOQOQOQOQOQ");
        }
    }
}
