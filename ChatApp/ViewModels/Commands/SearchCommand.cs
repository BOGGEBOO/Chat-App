using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModels.Commands
{
    internal class SearchCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private MainViewModel parent = null;
        public SearchCommand(MainViewModel parent)
        {
            this.parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            parent.SearchContacts();
        }
    }
}
