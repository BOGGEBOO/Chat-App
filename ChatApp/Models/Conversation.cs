using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.Models
{
    public class Conversation : INotifyPropertyChanged
    {
        private ObservableCollection<Message> messages;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ObservableCollection<Message> Messages
        {
            get { return messages; }
            set 
            { 
                messages = value;
                OnPropertyChanged("messages");
            }
        }
        public Conversation(ObservableCollection<Message> messages=null) 
        {
            this.messages = messages;
            if(this.messages == null)
            {
                this.messages = new ObservableCollection<Message>();
            }
        }

        public void add(Message message)
        {
            messages.Add(message);
            System.Diagnostics.Debug.WriteLine(messages.Count());
        }
    }
}
