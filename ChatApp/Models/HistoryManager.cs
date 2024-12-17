using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class HistoryManager
    {
        public List<Contact> Search(List<Contact> searchField, string searchString = "")
        {
            if(searchString == null)
            {
                searchString = "";
            }
            IEnumerable<Contact> queryResult = 
                from s in searchField
                where s.Name.Contains(searchString)
                select s;
            return queryResult.ToList();
        }

        public List<Contact> Read(string fileName)
        {
            string path = Directory.GetCurrentDirectory() + "\\..\\..\\..\\Data\\" + fileName + ".json";
            
            if(!File.Exists(path))
            {
                System.Diagnostics.Debug.WriteLine("NO FILE!!!");
                return new List<Contact>();
            }
            string historyString = File.ReadAllText(path);
            List<Contact> messages = JsonSerializer.Deserialize<List<Contact>>(historyString);
            return messages;
        }

        public void Write(List<Contact> conversations, string fileName)
        {
            string conversationHistory = JsonSerializer.Serialize(conversations);
            File.WriteAllText(Directory.GetCurrentDirectory() + "\\..\\..\\..\\Data\\" + fileName + ".json", conversationHistory);
        }
    }
}
