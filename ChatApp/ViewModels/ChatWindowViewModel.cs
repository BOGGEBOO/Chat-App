using ChatApp.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;



namespace ChatApp.ViewModels
{
    public class ChatWindowViewModel : BaseViewModel
    {

        //private ICommand AcceptConnection;

        public ChatWindowViewModel(string uName) 
        {
            System.Diagnostics.Debug.WriteLine(uName);
            UserName = uName;
        }
        private string userName;
        public string UserName
        {
            get => this.userName;
            set
            {
                this.userName = value;
            }
        }
        public ICommand AcceptConnection
        {
            get
            {
                if (AcceptConnection == null)
                {
                    AcceptConnection = new AcceptConnectionCommand(this);
                }
                return AcceptConnection;
            }
            set
            { AcceptConnection = value; }
        }



    }
}
