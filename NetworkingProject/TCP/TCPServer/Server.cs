using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
namespace TCPServer
{
    public class Server
    {
        TcpListener tcpListner;
        Thread connectThread;
        public List<Client> clients;
        Callback callback;
        ApplicationProtocol.ApplicationProtocol AppProtocol;
        int portNumber;

        public Server(int portNumber, Callback callback, ApplicationProtocol.ApplicationProtocol AppProtocol)
        {
            clients = new List<Client>();
            this.AppProtocol = AppProtocol;
            this.portNumber = portNumber;

            this.callback = callback;

            tcpListner = new TcpListener(IPAddress.Any, portNumber);
            tcpListner.Start();

            connectThread = new Thread(GetNewConnections);
            connectThread.Start();
        }

        public void GetNewConnections()
        {
            while (true)
            {
                TcpClient client = tcpListner.AcceptTcpClient();
                Console.WriteLine("New Client:" + client);
                Client newClient = new Client(client, this, callback);
                clients.Add(newClient);
                callback.ClientConnected(newClient);

            }
        }

        public void EndConnection(Client client)
        {
            client.SendTo(AppProtocol.CreateDisconnectMessage());
            client.client.Close();
            clients.Remove(client);
        }

        public void Broadcast(Client sender, string msg, bool sendToSelf = false)
        {
            foreach (Client c in clients)
            {
                if (c != sender || sendToSelf)
                {
                    c.SendTo(msg);
                }
            }
        }

        public void Broadcast(Client sender, byte[] msg, bool sendToSelf = false)
        {
            foreach (Client c in clients)
            {
                if (c != sender || sendToSelf)
                {
                    c.SendTo(msg);
                }
            }
        }
    }


    public class Client
    {
        public TcpClient client;
        Server server;
        Callback dataProcessor;
        public Client(TcpClient client, Server server, Callback dataProcessor)
        {
            this.dataProcessor = dataProcessor;
            this.client = client;
            this.server = server;
            new Thread(Listen).Start();
        }

        public void Listen()
        {
            while (true)
            {
                if (!client.Connected) { return; }
                try
                {
                    byte[] bytesFrom = new byte[65535];
                    NetworkStream networkStream = client.GetStream();
                    networkStream.Read(bytesFrom, 0, bytesFrom.Length);
                    ProcessMessage(this, bytesFrom);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    //server.EndConnection(this);
                    //return;
                }
            }
        }

        public void SendTo(string msg)
        {
            client.Write(msg);
        }

        public void SendTo(byte[] msg)
        {
            client.Write(msg);
        }

        public void Close()
        {
            server.EndConnection(this);
        }

        private void ProcessMessage(Client client,byte[] msg)
        {
            dataProcessor.Process(client,msg);
        }
    }

    public abstract class Callback
    {
        abstract public void Process(Client client, byte[] msg);
        abstract public void ClientConnected(Client client);
    }

    static class TCPExtension
    {
        public static void Write(this TcpClient tcpClient, string msg)
        {
            if (!tcpClient.Connected) { return; }
            byte[] bmsg = Encoding.ASCII.GetBytes(msg);//Convert header to bytes
            var stream = tcpClient.GetStream();
            stream.Write(bmsg, 0, bmsg.Length);
            stream.Flush();
        }

        public static void Write(this TcpClient tcpClient, byte[] data)
        {
            if (!tcpClient.Connected) { return; }
            var stream = tcpClient.GetStream();
            stream.Write(data, 0, data.Length);
            stream.Flush();
        }

        public static string ReadString(this TcpClient tcpClient)
        {
            var bytes = new byte[tcpClient.ReceiveBufferSize];
            var stream = tcpClient.GetStream();
            stream.Read(bytes, 0, tcpClient.ReceiveBufferSize);
            var msg = Encoding.ASCII.GetString(bytes);
            return msg.Substring(0, msg.IndexOf("\0", StringComparison.Ordinal));
        }
    }
}
