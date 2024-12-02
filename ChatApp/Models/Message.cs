using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class Message
    {
        private string type;
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string sender;
        public string Sender
        {
            get { return sender; }
            set { sender = value; }
        }

        private string date;
        public string Date
        { 
            get { return date; } 
            set {  date = value; } 
        }

        private string content;
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        public Message(string type, string sender, string date, string content="")
        {
            this.type = type;
            this.sender = sender;
            this.date = date;
            this.content = content;
        }
    }
}
