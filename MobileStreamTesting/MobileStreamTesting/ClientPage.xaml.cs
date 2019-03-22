using FFImageLoading.Forms;
using MobileStreamTesting.UDPServer;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileStreamTesting
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ClientPage : ContentPage
	{
        public static ClientPage clientPage;
        public static CachedImage previewImage;
        public static SKCanvasView canvasView;
        public static UDPSocket c,s;
        public static int frame = 0;

		public ClientPage ()
		{
            clientPage = this;
			InitializeComponent ();
            //previewImage = PreviewImage;
            canvasView = CanvasView;
            UDPServer.NetworkDataHandler.InitializeNetworkPackages();
            c = new UDPSocket();
            s = new UDPSocket();
            c.Client("71.80.130.81", 8082);
            //c.Client("23.99.224.56", 443);
            //s.Server(GetLocalIP(), ((IPEndPoint)c._socket.LocalEndPoint).Port);
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int)ClientPackets.Image);
            buffer.WriteString("gimme");
            c.Send(buffer.ToArray());
        }

        public void RefreshStream(object sender ,EventArgs e)
        {
            frame = 0;
        }

        public string GetLocalIP()
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
        public string GetRemoteIP()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.RemoteEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }
        public SKBitmap CameraPreview { get { return cameraPreview; } set { cameraPreview = value; OnCameraPreviewChange(); } }
        private SKBitmap cameraPreview;

        public void OnCameraPreviewChange()
        {
            Debug.WriteLine("frame updated");
            Device.BeginInvokeOnMainThread(()=>canvasView.InvalidateSurface());
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();
            if (CameraPreview != null)
            {
                using (var paint = new SKPaint())
                {
                    Debug.WriteLine("Painting canvas");
                    // clear the canvas / fill with black
                    canvas.DrawColor(SKColors.Black);
                    float scale = Math.Min((float)info.Width / CameraPreview.Width,
                               (float)info.Height / CameraPreview.Height);
                    float x = (info.Width - scale * CameraPreview.Width) / 2;
                    float y = (info.Height - scale * CameraPreview.Height) / 2;
                    SKRect destRect = new SKRect(x, y, x + scale * CameraPreview.Width,
                                                       y + scale * CameraPreview.Height);
                    canvas.DrawBitmap(CameraPreview,
                        destRect, paint);
                }
            }
        }
    }
}