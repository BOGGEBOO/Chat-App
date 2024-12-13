using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;

namespace ChatApp.Models
{
    public class NetworkManager : INotifyPropertyChanged
    {

        private TcpClient client;
        private TcpListener server;
        private Thread thread;
        private StreamReader reader;
        private StreamWriter writer;
        private bool connectionAccepted;
        public bool ConnectionAccepted
        { 
            get { return connectionAccepted; }
            set 
            { 
                connectionAccepted = value;
                OnPropertyChanged(connectionAccepted.ToString());
            } 
        }
        private bool connected;
        public bool Connected
        {
            get { return connected; }
            set 
            { 
                connected = value;
                OnPropertyChanged(connected.ToString());
            }
        }
        private string sender = "";
        
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

        private Message lastMessage;
        public Message LastMessage
        {
            get { return lastMessage; }
            set { lastMessage = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            
        }

        private void NotifyStatusChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void NotifyMessageChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void startServer(string host, int port)
        {

            try
            {
                Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        connected = true;
                        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
                        server = new TcpListener(ipEndPoint);
                        
                        while (!ConnectionAccepted)
                        {
                            server.Start();
                            System.Diagnostics.Debug.WriteLine("Start listening...");

                            client = server.AcceptTcpClient();
                            thread = new Thread(listen);
                            thread.IsBackground = true;
                            thread.Start();

                            reader = new StreamReader(client.GetStream());
                            writer = new StreamWriter(client.GetStream());

                            System.Diagnostics.Debug.WriteLine("Connection accepted!");

                        }
                    }
                    catch (SocketException ex)
                    {
                        if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                        {
                            NotifyStatusChanged("busy_adress");
                            //return "The address is already in use.";
                        }
                        else
                        {
                            NotifyStatusChanged("no_start");
                            System.Diagnostics.Debug.WriteLine(ex.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        NotifyStatusChanged("no_start");
                    }
                });
                //return "200 OK";
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
                {
                    NotifyStatusChanged("busy_adress");
                    //return "The address is already in use.";
                }
                else
                {
                    NotifyStatusChanged("no_start");
                    //return "Error starting server";
                }
            }
            catch (Exception e)
            {
                NotifyStatusChanged("no_start");
                //return "Error starting server";
            }
        }

        public void startClient(string host, int port)
        {
            try
            {
               
                Task.Factory.StartNew(async () =>
                {
                    connected = true;
                    
                    try
                    {
                        client = new TcpClient(); //inte ha host, port här, skapar en connection då...


                        client.Connect(host, port);
                        thread = new Thread(listen);
                        thread.IsBackground = true;
                        thread.Start();

                        writer = new StreamWriter(client.GetStream());
                        reader = new StreamReader(client.GetStream());

                        Message connection_request = new Message("connection_request", username, DateTime.Now.ToString("g"));
                        sendMessage(connection_request);
                    }
                    catch(SocketException ex)
                    {
                        NotifyStatusChanged("no_listener");
                        
                    }
                    catch (Exception ex)
                    {
                        NotifyStatusChanged("connection_error");
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                       
                    }
                });
                
                
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                
            }
        }

        public void sendMessage(Message message)
        {
            try
            {
                if (client.Connected)
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

        public void listen()
        {
            while (client.Connected)
            {
                try
                {
                    string data = reader.ReadLine();

                    if (data != null)
                    {
                        Message message = JsonSerializer.Deserialize<Message>(data);
                        lastMessage = message;
                        App.Current.Dispatcher.Invoke(() => { NotifyMessageChanged(message.Type); });
                    }
                }
                catch (Exception e)
                {
                    Message message = new Message("dramatic_disconnect", sender, DateTime.Now.ToString("g"));
                    lastMessage = message;
                    App.Current.Dispatcher.Invoke(() => { NotifyMessageChanged(message.Type); });
                    
                }
            }
        }

        public void Disconnect()
        {
            if (client != null && client.Connected)
            {
                Connected = false;
                Message message = new Message("disconnect", username, DateTime.Now.ToString("g"));
                sendMessage(message);

                Close();
            }
        }

        public void Close()
        {
            writer.Close();
            reader.Close();
            client.Close();
            Connected = false;
        }
    }
}
