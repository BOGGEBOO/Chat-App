                using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using ChatApp.Views;
using ChatApp.ViewModels;
using System.Text.Json;
using System.Windows.Documents;

namespace ChatApp.Models
{
    internal class NetworkManager : INotifyPropertyChanged
    {

        private TcpClient client;
        private TcpClient endPoint;
        private TcpClient pendingClient;
        private TcpListener server;
        // private NetworkStream stream;

        private StreamReader reader;
        private StreamWriter writer;

        public bool connectionAccepted;

        private string sender;

        private bool listening;

        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public event EventHandler<ShowAcceptMessageBoxEventArgs> showAcceptMessageBox;

        // private string pendingClientName;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            }
        }

        public string startServer(string host, int port)
        {
             
            try
            {
                Task.Factory.StartNew(async () =>
                {
                    
                        

                    try
                    {
                        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
                        server = new TcpListener(ipEndPoint);
                        while (true)
                        {

                            server.Start();
                        System.Diagnostics.Debug.WriteLine("Start listening...");
                        
                        
                        client = server.AcceptTcpClient();

                        reader = new StreamReader(client.GetStream());
                        writer = new StreamWriter(client.GetStream());

                        System.Diagnostics.Debug.WriteLine("Connection accepted!");

                        if (acceptConnection())
                        {
                            break;
                    }
                       
                    }
                    }

                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    

                    }
                
                });
                return "200 OK";
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    System.Diagnostics.Debug.WriteLine("The address is already in use.");
                    return "The address is already in use.";
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Error starting server: " + ex.Message);
                    return "Error starting server";
                }
            }
            catch (Exception e)
            {
                return "Error starting server";
            }

        }


        private bool acceptConnection()
        {
            
            while (true)
            {
                string data = reader.ReadLine();

                if(data != null)
                {
                    Message message = JsonSerializer.Deserialize<Message>(data);
                    
                    if(message.Type == "connection")
                    {
                        sender = message.Sender;
                        break;
                    }
                }
            }
            showAcceptMessageBox?.Invoke(null, new ShowAcceptMessageBoxEventArgs("Chat with " + sender + "?"));
            while (true) { 
                if(connectionAccepted)
                {
                    Message message = new Message("connection", username, DateTime.Now.ToString(), "ACCEPTED");
                    sendMessage(message);
                    return true;
                }
                else
                {

                    Message message = new Message("connection", username, DateTime.Now.ToString(), "DENIED");
                    sendMessage(message);
                    writer.Close();
                    reader.Close();
                    client.Dispose();
                    client.Close();
                    return false;
                }
            }
        }
        
        public void Disconnect()
        {
            if(client != null && client.Connected)
            {
                Message message = new Message("disconnect", username, DateTime.Now.ToString());
                sendMessage(message);
                writer.Close();
                reader.Close();
                
                client.Close();
                
            }
            
        }
        public void StopListening(Views.ConnectionRequest request)
        {
            /*if (server != null)
            {
                server.Stop();
                if (pendingClient != null)
                {
                    if (request.ShowMessageBox(pendingClientName))
                    {
                        MainWindowViewModel.Instance.AddContact(pendingClientName, pendingClient);
                    }
                    else
                    {
                        pendingClient.Client.Send(Encoding.UTF8.GetBytes("DENIED"));
                        pendingClient.Close();
                    }
                }
            }*/
        } 

            public string startClient(string host, int port)
        {
            string errorMessage = "Error connecting to server. Check your information. ";
            try
            {
                client = new TcpClient(); //inte ha host, port här, skapar en connection då...

                while(!client.Connected)
                {
                    client.Connect(host, port);
                }

                // stream = client.GetStream();

                writer = new StreamWriter(client.GetStream());
                reader = new StreamReader(client.GetStream());
                
                Message connection_request = new Message("connection", username, DateTime.Now.ToString());
                sendMessage(connection_request);
                Message response = receiveMessage();

                if(response.Content == "DENIED" && response.Type == "connection")
                {
                    writer.Close();
                    reader.Close();
                    client.Close();
                    return response.Content;
                    
                }
                
            } 
            catch (SocketException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return "Error when connecting";
               
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                
                return errorMessage;
            }
            return "200 OK";
        }

        public void sendMessage(Message message)
        {
            try
            {
                if(client.Connected)
                {
                    string jsonS = JsonSerializer.Serialize<Message>(message);
                    
                    writer.WriteLine(jsonS);
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        public Message receiveMessage()
        {
            while (true)
            {
                string data = reader.ReadLine();

                if (data != null)
                {
                    Message message = JsonSerializer.Deserialize<Message>(data);

                    if (message.Type == "connection")
                    {
                        sender = message.Sender;
                        if(message.Content == "ACCEPTED")
                        {
                            // gör nåt
                        }
                        else if (message.Content == "DENIED")
                        {
                            // skriv ut meddelande
                            // fett dissad yao
                            return message;
                        }
                        break;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(message.Type);
                        break;
                    }
                }
            }
            return new Message("", "", "");
        }

       /* private void handleConnection(TcpClient endPoint)
        {
            stream = endPoint.GetStream();
            var buffer = new byte[1024];

            while (true)
            {
                int received = stream.Read(buffer, 0, 1024);
                if (received == 0)
                    break;
                var message = Encoding.UTF8.GetString(buffer, 0, received);
                System.Diagnostics.Debug.WriteLine(message);
            }
        } */
    

        public void close()
        {
            client.Close();
            // stream.Close();
        }


    }
    public class ShowAcceptMessageBoxEventArgs : EventArgs
    {
        public ShowAcceptMessageBoxEventArgs(string s)
        {
            text = s;
        }

        public string text { get; set; }
    }


}
