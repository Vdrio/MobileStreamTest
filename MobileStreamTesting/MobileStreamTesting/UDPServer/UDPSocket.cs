using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobileStreamTesting.UDPServer
{
    public class UDPSocket
    {
        public Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private const int bufSize = 51200;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public void Server(string address, int port)
        {
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            Receive();
        }

        //UdpClient udpClient = new UdpClient(10000);
        public void Client(string address, int port)
        {
            _socket.Connect(IPAddress.Parse(address), port);
            _socket.EnableBroadcast = true;
            //udpClient.Connect(IPAddress.Parse(address), port);
            Thread thread = new Thread(new ThreadStart(ReceiveClient));
            thread.IsBackground = true;
            thread.Start();
            //ReceiveClient();
            //Receive();
        }

        public void Send(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);
                Console.WriteLine("SEND: {0}, {1}", bytes, text);
            }, state);
        }

        public Task Send(byte[] bytes)
        {
            return Task.Run(() =>
            {
                _socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, (ar) =>
                {
                    State so = (State)ar.AsyncState;
                    int byteNum = _socket.EndSend(ar);
                    //Console.WriteLine("SEND: {0}, {1}", byteNum, text);
                }, state);
            });
        }

        private void Receive()
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                //Debug.WriteLine("Received data!");
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                NetworkDataHandler.HandleNetworkInformation(so.buffer);
                _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
                //Console.WriteLine("RECV: {0}: {1}, {2}", epFrom.ToString(), bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes));
            }, state);
        }

        private void ReceiveClient()
        {
            while (true)
            {
                byte[] buffer = new byte[51200];
                _socket.ReceiveFrom(buffer, ref epFrom);
                NetworkDataHandler.HandleNetworkInformation(buffer);
            
               // IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 10000);
                //byte[] content = udpClient.Receive(ref remoteIPEndPoint);
               // NetworkDataHandler.HandleNetworkInformation(content);
            }
        }

        public static string GetLocalIPAddress()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }

        public static string GetRemoteIPAddress()
        {
            string remoteIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.RemoteEndPoint as IPEndPoint;
                remoteIP = endPoint.Address.ToString();
            }
            return remoteIP;
        }
    }
}
