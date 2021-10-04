using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Windows;

namespace Couple
{
    public class TcpServer
    {
        static TcpListener server;
        static Task<TcpClient> connect;
        static List<Client> clients = new List<Client>();
        public static bool running = false;

        public static void Listen()
        {
            int port = 13000;
            IPAddress localaddr = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(localaddr, port);
            server.Start();

            connect = server.AcceptTcpClientAsync();
            running = true;
        }
        public static void CheckNewClient()
        {
            if (connect.IsCompletedSuccessfully)
            {
                Client newClient = new Client();
                newClient.client = connect.Result;
                newClient.point = null;
                newClient.task = SendToClient("0, 0\0", newClient);
                connect = server.AcceptTcpClientAsync();

                clients.Add(newClient);
            }
        }
        public static string CheckClientData()
        {
            string serverPointTo = null;
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].task.IsCompleted)
                {
                    string result = Encoding.ASCII.GetString(clients[i].memory);
                    clients[i].point = result;
                    if (i == 0)
                    {
                        clients[i].task = SendToClient(MainWindow.windowPoint, clients[i]);
                    }
                    else
                    {
                        clients[i].task = SendToClient(clients[i - 1].point, clients[i]);
                    }
                    if (i == clients.Count - 1)
                    {
                        serverPointTo = clients[i].point;
                    }
                }
            }
            return serverPointTo;
        }
        public static ValueTask<int> SendToClient(string str, Client client)
        {
            NetworkStream stream = client.client.GetStream();

            byte[] msg = Encoding.ASCII.GetBytes(str);
            byte[] data = new byte[256];

            msg.CopyTo(data, 0);
            try
            {
                stream.Write(data, 0, data.Length);
            }
            catch
            {
                clients.Remove(client);
            }
            return stream.ReadAsync(client.memory);
        }
        public class Client
        {
            public TcpClient client;
            public string point;

            public ValueTask<int> task;
            public byte[] memory = new byte[256];
        }
    }
}
