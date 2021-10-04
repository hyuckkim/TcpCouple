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
    public class TcpUser
    {
        public static byte[] buffer = new byte[256];
        public static NetworkStream stream;
        public static ValueTask<int>? task;

        public static TcpClient client;
        public static void Connect()
        {
            string server = "127.0.0.1";
            int port = 13000;
            client = new TcpClient(server, port);
            stream = client.GetStream();
            task = stream.ReadAsync(buffer);
        }
        public static void SendToServer(string str)
        {
            byte[] msg = Encoding.ASCII.GetBytes(str);
            byte[] data = new byte[256];

            msg.CopyTo(data, 0);

            try
            {
                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Application.Current.Shutdown();
            }
            task = stream.ReadAsync(buffer);
        }
    }
}
