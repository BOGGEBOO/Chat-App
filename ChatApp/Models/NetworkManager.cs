using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using ChatApp.Views;
using ChatApp.ViewModels;
using System.Text.Json;
using System.Windows.Documents;
using System.Windows;
using Microsoft.VisualBasic;
using System.Media;
using System.Reflection.PortableExecutable;
using System.Collections.ObjectModel;

namespace ChatApp.Models
{
    public class NetworkManager : INotifyPropertyChanged
    {

        private TcpClient client;
        private TcpListener server;
        private Thread thread;
        private StreamReader reader;
        private StreamWriter writer;
        public bool connectionAccepted;
        private bool connected;
        private string sender;
        private ChatWindowViewModel viewModel;
        public ChatWindowViewModel ViewModel
        {
            get { return viewModel; }
            set { viewModel = value; }
        }
        public string Sender
        { 
            get { return sender; } 
            set { sender = value; } 
        }
        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        

        public event EventHandler<MessageRecievedEventArgs> messageRecieved;
        public event EventHandler<ShowAcceptMessageBoxEventArgs> showAcceptMessageBox;
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            /*if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

            } */
        }

        public NetworkManager()
        {
            messageRecieved += messageRecievedHandler;
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
            server.Stop();
            showAcceptMessageBox?.Invoke(null, new ShowAcceptMessageBoxEventArgs("Chat with " + sender + "?"));
            while (true) { 
                if(connectionAccepted)
                {
                    Message message = new Message("connection", username, DateTime.Now.ToString(), "ACCEPTED");
                    sendMessage(message);

                    thread = new Thread(listen);
                    thread.IsBackground = true;
                    thread.Start();

                    connected = true;
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
                    connected = false;
                    return false;
                }
            }
        }
        
        public void Disconnect()
        {
            if(client != null && client.Connected)
            {
                connected = false;
                Message message = new Message("disconnect", username, DateTime.Now.ToString());
                sendMessage(message);

                writer.Close();
                reader.Close();
                client.Close();
                connected = false;
            }
        }

        public void sendMessage(Message message)
        {
            try
            {
                if (client.Connected)
                {

                    /*if(viewModel != null)
                    {
                        viewModel.Conversation.Add(message);
                    }*/
                    
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

        public void listen()
        {
            while(client.Connected)
            {
                try
                {
                    if (connected)
                    {
                        string data = reader.ReadLine();

                        if (data != null)
                        {
                            Message message = JsonSerializer.Deserialize<Message>(data);
                            if (message.Type == "disconnect")
                            {
                                MessageBox.Show(message.Sender + " has disconnected due to suffering from dead.", "whoop", MessageBoxButton.OK);
                                writer.Close();
                                reader.Close();
                                client.Close();
                                connected = false;
                                return;
                            }
                            else if (message.Type == "chat_message")
                            {
                                App.Current.Dispatcher.Invoke(() => { viewModel.Conversation.Add(message); });
                                
                            }

                        }

                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                    MessageBox.Show("Your frien has disconnected due to suffering from unnatural deadness.", "whoop", MessageBoxButton.OK);
                }
            }
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
                            
                            thread = new Thread(listen);
                            thread.IsBackground = true;
                            thread.Start();
                            connected = true;
                        }
                        else if (message.Content == "DENIED")
                        {
                            connected = false;
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


    
        void messageRecievedHandler(object sender, MessageRecievedEventArgs args)
        {
            if (args.message.Type == "disconnect")
            {
                MessageBox.Show(args.message.Sender + " has disconnected due to suffering from dead.", "whoop", MessageBoxButton.OK);
                writer.Close();
                reader.Close();
                client.Close();
                connected = false;
                return;
            }
            else if (args.message.Type == "chat_message")
            {

                viewModel.Conversation.Add(args.message);
            }           

            //Action<Message> addMethod = conversation.Add;
            //Application.Current.Dispatcher.BeginInvoke(addMethod, args.message);
        }

    }
    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageRecievedEventArgs(Message m)
        {
            message = m;
        }

        public Message message { get; set; }
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
