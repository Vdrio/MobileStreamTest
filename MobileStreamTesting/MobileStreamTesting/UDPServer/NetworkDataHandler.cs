using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace MobileStreamTesting.UDPServer
{
    public class NetworkDataHandler
    {
        private delegate void PacketData(byte[] data);
        private static Dictionary<int, PacketData> Packets;

        public static void InitializeNetworkPackages()
        {
            Console.WriteLine("Initializing network packages...");
            Packets = new Dictionary<int, PacketData>
            {
                { (int)ServerPackets.ConnectionOk, HandleConnectionOK }, { (int)ServerPackets.StringMessage, HandleStringMessage}
                , {(int)ServerPackets.Image, HandleImage}, {(int)ServerPackets.VideoFrame, HandleVideoFrame},
                {(int)ServerPackets.Audio, HandleAudio }
            };
        }

        public static void HandleNetworkInformation(byte[] data)
        {
            //Debug.WriteLine("Handling network info");
            int packetNum;
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            packetNum = buffer.ReadInteger();
            buffer.Dispose();
            if (Packets.TryGetValue(packetNum, out PacketData Packet))
            {
                Packet.Invoke(data);
            }
        }

        private static void HandleConnectionOK(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string msg = buffer.ReadString();
            buffer.Dispose();
            //Debug.WriteLine(msg);
            UDPClient.ThankYouServer();
            if (UDPClient.OnStringMessageReceived != null)
            {
                UDPClient.OnStringMessageReceived.Invoke(msg);
            }
        }

        private static void HandleStringMessage(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string msg = buffer.ReadString();
            buffer.Dispose();
            Console.WriteLine(msg);
            if (UDPClient.OnStringMessageReceived != null)
            {
                UDPClient.OnStringMessageReceived.Invoke(msg);
            }
        }

        private static void HandleAudio(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string msg = buffer.ReadString();
            buffer.Dispose();
            Console.WriteLine(msg);
        }

        private static void HandleImage(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string msg = buffer.ReadString();
            buffer.Dispose();
            //Console.WriteLine(msg);
        }

        private static void HandleVideoFrame(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            int frame = buffer.ReadInteger();
            SKBitmap msg = buffer.ReadJpegToBitmap(data.Length - 8);
            buffer.Dispose();
            if ((App.Current.MainPage as NavigationPage).CurrentPage is ClientPage)
            {
                if (ClientPage.canvasView != null && frame > ClientPage.frame)
                {
                    ClientPage.clientPage.CameraPreview = msg;
                    //ClientPage.canvasView.InvalidateSurface();
                    //ClientPage.previewImage.CacheDuration = TimeSpan.FromSeconds(5);
                    ClientPage.frame = frame;
                }
            }
            else if ((App.Current.MainPage as NavigationPage).CurrentPage is VideoChatPage)
            {
                ClientPage.frame = frame;
                VideoChatPage.videoChatPage.CameraClientPreview = msg;
            }
            //System.Diagnostics.Debug.WriteLine("Successfully loaded image");
        }
    }
}
