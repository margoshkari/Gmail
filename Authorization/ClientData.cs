using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public static class ClientData
    {
        public static byte[] data;
        public static Socket socket;
        public static IPEndPoint iPEndPoint;
        static ClientData()
        {
            data = new byte[256];
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
        }
        public static string GetMsg()
        {
            int bytes = 0;
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                do
                {
                    bytes = socket.Receive(data);
                    stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (socket.Available > 0);
            }
            catch (Exception ex) { }
            return stringBuilder.ToString();
        }
    }
}