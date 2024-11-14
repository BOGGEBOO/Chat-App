using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.ViewModels
{
    class ChatWindowViewModel
    {

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
    }
}
