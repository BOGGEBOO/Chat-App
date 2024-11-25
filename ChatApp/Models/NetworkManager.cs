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

namespace ChatApp.Models
{
    internal class NetworkManager : INotifyPropertyChanged
    {

        private TcpClient client;
        private TcpClient endPoint;
        private TcpClient pendingClient;
        private TcpListener server;
        private NetworkStream stream;

        private string pendingClientName;

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
                    IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
                    server = new TcpListener(ipEndPoint);

                    try
                    {
                        server.Start();
                        System.Diagnostics.Debug.WriteLine("Start listening...");
                        
                        
                        client = server.AcceptTcpClient();
                        
                       
                        System.Diagnostics.Debug.WriteLine("Connection accepted!");
                        stream = client.GetStream();
                       

                    }
                
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    

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

            // return "200 OK";
        }

        public void close() 
        {
            client.Close();
            stream.Close();
        }

        public void StopListening(Views.ConnectionRequest request)
        {
            if (server != null)
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

                stream = client.GetStream();
                
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

        private void handleConnection(TcpClient endPoint)
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
        }
    }

    
}
