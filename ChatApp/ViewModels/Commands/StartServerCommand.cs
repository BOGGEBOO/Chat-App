using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using ChatApp.ViewModels;

namespace ChatApp.ViewModels.Commands
{
    internal class StartServerCommand : ICommand
    {

        public event EventHandler CanExecuteChanged;
        private tempViewModel parent = null;
        public StartServerCommand(tempViewModel parent) 
        {
            this.parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parent.ValidateFields(out string errorMessage))
            {
                string isSuccessful = parent.NetworkManager.startServer(parent.IpAddress, Int32.Parse(parent.PortNumber));
                if(isSuccessful == "200 OK")
                {
                    parent.ValidationMessage = "";
                    //parent.startGameBoard();
                }
                else
                {
                    parent.ValidationMessage += isSuccessful;
                }
                
            }
            else
            {
                parent.ValidationMessage += errorMessage;
            }
        }
    }
}
