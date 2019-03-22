using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace MobileStreamTesting.UDPServer
{
    public class UDPClient
    {

        public delegate void StringMessageReceived(string data);
        public delegate void ActionStatusUpdate(string data);
        public static StringMessageReceived OnStringMessageReceived;
        private static Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private byte[] AsyncBuffer = new byte[51200];

        public static void ConnectToServer()
        {
            Console.WriteLine("Connecting to server...");
            ClientSocket.BeginConnect("192.168.1.101", 5555, new AsyncCallback(ConnectCallback), ClientSocket);
        }

        public static void ConnectToServer(string ip, int port)
        {
            Console.WriteLine("Connecting to server...");
            ClientSocket.BeginConnect(ip, port, new AsyncCallback(ConnectCallback), ClientSocket);
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            ClientSocket.EndConnect(ar);
            while (ClientSocket != null)
            {
                OnReceive();
            }
        }

        private static void CompletedLargeDataTransfer(IAsyncResult ar)
        {

        }

        private static void OnReceive()
        {
            byte[] sizeInfo = new byte[4];
            byte[] receivedBuffer = new byte[51200];
            int totalRead = 0, currentRead = 0;



            try
            {
                currentRead = totalRead = ClientSocket.Receive(sizeInfo);
                if (totalRead <= 0)
                {
                    Console.WriteLine("Not connected to server.");
                }
                else
                {
                    Console.WriteLine("Reading data...");
                    while (totalRead < sizeInfo.Length && currentRead > 0)
                    {
                        currentRead = ClientSocket.Receive(sizeInfo, totalRead, sizeInfo.Length - totalRead, SocketFlags.None);
                        totalRead += currentRead;
                    }

                    int messageSize = 0;
                    messageSize |= sizeInfo[0];
                    messageSize |= (sizeInfo[1] << 8);
                    messageSize |= (sizeInfo[2] << 16);
                    messageSize |= (sizeInfo[3] << 24);

                    byte[] data = new byte[messageSize];
                    totalRead = 0;
                    DateTime timeStart = DateTime.Now;
                    var asyncResult = ClientSocket.BeginReceive(data, totalRead, data.Length - totalRead, SocketFlags.None, new AsyncCallback(CompletedLargeDataTransfer), null);
                    asyncResult.AsyncWaitHandle.WaitOne(66);
                    //Client can handle stream
                    if (asyncResult.IsCompleted)
                    {

                        try
                        {
                            int dataSize = ClientSocket.EndReceive(asyncResult);
                            // EndReceive worked and we have received data and remote endpoint
                        }
                        catch (Exception ex)
                        {
                            // EndReceive failed and we ended up here
                        }

                        //if true, attempt to upgrade stream quality
                        if (DateTime.Now - timeStart <= TimeSpan.FromMilliseconds(44))
                        {

                        }
                    }
                    //Client can't handle stream, request lower quality
                    else
                    {
                        // The operation wasn't completed before the timeout and we're off the hook
                    }
                    currentRead = totalRead = ClientSocket.Receive(data, totalRead, data.Length - totalRead, SocketFlags.None);
                    while (totalRead < messageSize && currentRead > 0)
                    {
                        currentRead = ClientSocket.Receive(data, totalRead, data.Length - totalRead, SocketFlags.None);
                        totalRead += currentRead;
                    }

                    NetworkDataHandler.HandleNetworkInformation(data);


                }
            }
            catch
            {
                Console.WriteLine("Not connected to server.");
            }
        }

        public static void SendData(byte[] data)
        {
            ClientSocket.Send(data);
        }

        public static void ThankYouServer()
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int)ClientPackets.ThankYou);
            buffer.WriteString("Thank you for letting me connect");
            SendData(buffer.ToArray());
            buffer.Dispose();
        }

    }
}
