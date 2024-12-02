using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.ViewModels.Commands
{
    internal class StartClientCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private MainWindowViewModel parent = null;
        // private readonly MainWindowViewModel _viewModel;
        public StartClientCommand(MainWindowViewModel parent)
        {
            this.parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            if(parent.ValidateFields(out string errorMessage)) 
            {
                string isSuccessfull = parent.NetworkManager.startClient(parent.IpAddress, Int32.Parse(parent.PortNumber));
                if (isSuccessfull == "200 OK")
                {
                    // öppna chatfönster
                    parent.ValidationMessage = "";
                    parent.startClientBoard();
                } 
                else if(isSuccessfull == "DENIED")
                {
                    parent.ValidationMessage = "Server denied connection";
                }
                else
                {
                    parent.ValidationMessage = isSuccessfull;
                }
            }
            else
            {
                parent.ValidationMessage = errorMessage;
            }
            

            

        }
    }
}
