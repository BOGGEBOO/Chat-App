using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class Contact
    {
        private string name;
        public string Name 
        { 
            get => name;
            set => name = value; 
        }

        private string lastActive;
        public string LastActive
        {
            get => lastActive;
            set => lastActive = value;
        }

        private List<Message> messages;
        public List<Message> Messages 
        { 
            get => messages; 
            set => messages = value; 
        }

        public Contact(string name, string lastActive, List<Message> messages) 
        {
            this.name = name;
            this.lastActive = lastActive;
            this.messages = messages;
        }
    }
}
