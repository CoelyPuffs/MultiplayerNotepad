using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

namespace TCPClient
{
    [Serializable]
    public class keyUpdate
    {
        public string data;
        public int cursorLocation;
        public int highlightLength;
        public int extras;
        public keyUpdate(string data, int cursorLocation, int highlightLength, int extras)
        {
            this.data = data;
            this.cursorLocation = cursorLocation;
            this.highlightLength = highlightLength;
            this.extras = extras;
        }
    }

    public class Client
    {
        TcpClient client;
        Thread listenThread;
        DataProcessor dataProcessor;
        string hostIP;
        int hostPortNumber;
        ApplicationProtocol.ApplicationProtocol AppProtocol;
        public Client(string hostIP, int hostPortNumber, DataProcessor dataProcessor, ApplicationProtocol.ApplicationProtocol AppProtocol)
        {
            client = new TcpClient();
            this.dataProcessor = dataProcessor;
            this.hostIP = hostIP;
            this.hostPortNumber = hostPortNumber;
            this.AppProtocol = AppProtocol;

            try
            {
                client.Connect(hostIP, hostPortNumber);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            listenThread = new Thread(Listener);
            listenThread.Start();
        }

        public void Send(keyUpdate msg)
        {
            byte[] extras = BitConverter.GetBytes(msg.extras);
            byte[] cursorPosition = BitConverter.GetBytes(msg.cursorLocation);
            byte[] highlightLength = BitConverter.GetBytes(msg.highlightLength);
            byte[] data = Encoding.ASCII.GetBytes(msg.data);
            byte[] fullData = extras.Concat(cursorPosition.Concat(highlightLength.Concat(data).ToArray()).ToArray()).ToArray();
            client.Write(fullData);
        }

        public void Send(byte[] data)
        {
            client.Write(data);
        }

        public void Close()
        {
            //keyUpdate disconnectMSG = new keyUpdate("", 0, 0, 9);
            //Send(disconnectMSG);
            Send(AppProtocol.CreateDisconnectMessage());
            client.Close();
        }

        public void Listener()
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
                    client.Close();
                    Console.WriteLine(e.Message);
                    //throw e;
                    return;
                }
            }
        }

        private void ProcessMessage(Client client,byte[] msg)
        {
            dataProcessor.Process(client,msg);
        }
    }

    public abstract class DataProcessor
    {
        abstract public void Process(Client client,byte[] msg);
    }

    static class TCPExtension
    {
        public static void Write(this TcpClient tcpClient, byte[] bmsg)
        {
            if (!tcpClient.Connected) { return; }
            //byte[] bmsg = Encoding.ASCII.GetBytes(msg);//Convert header to bytes
            var stream = tcpClient.GetStream();
            stream.Write(bmsg, 0, bmsg.Length);
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
