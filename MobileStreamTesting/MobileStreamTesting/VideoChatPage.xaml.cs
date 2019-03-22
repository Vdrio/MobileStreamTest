using MobileStreamTesting.ImageCorrection;
using MobileStreamTesting.UDPServer;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileStreamTesting
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VideoChatPage : ContentPage
	{
        int frame = 1;
        UDPSocket c;
        //public CachedImage cameraImage;
        public static byte[] takenImage;
        public CameraPrev.CameraPreview camPreview;
        public static VideoChatPage videoChatPage;
        public SKCanvasView canvasView;

        public VideoChatPage()
        {
            videoChatPage = this;
            InitializeComponent();
            canvasView = PreviewCanvas;
            camPreview = CameraPreviewView;
            CameraPreviewView.PictureFinished += OnPictureFinished;
            if (CameraIsFront)
            {
                StartingCameraIsFront = true;
            }
            else
            {
                StartingCameraIsFront = false;
            }
            if (CameraPreviewView.CameraClick != null ? CameraPreviewView.CameraClick.CanExecute(null) : false)
            {
                //CameraPreviewView.CameraClick?.Execute(null);
            }
            //PreviewImage.Source = ImageSource.FromStream();
            UDPServer.NetworkDataHandler.InitializeNetworkPackages();
            c = new UDPSocket();
            c.Client("71.80.130.81", 8082);
            canvasView = ClientCanvas;
            UDPServer.NetworkDataHandler.InitializeNetworkPackages();
            //c = new UDPSocket();
            //s = new UDPSocket();
            //c.Client("71.80.130.81", 8082);
            //c.Client("23.99.224.56", 443);
            //s.Server(GetLocalIP(), ((IPEndPoint)c._socket.LocalEndPoint).Port);
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int)ClientPackets.Image);
            buffer.WriteString("gimme");
            c.Send(buffer.ToArray());
            //ChangeCamera(false);
        }

        public async void StartFrameSending()
        {
            Device.StartTimer(TimeSpan.FromSeconds(.1), SendFrameFunc);
        }

        public bool CameraIsFront = true;
        public async void ChangeCamera(bool front = true)
        {
            await Task.Delay(1500);
            PreviewGrid.Children.Remove(CameraPreviewView);
            App.cameraPreview.ChangeCamera(front);
            CameraIsFront = front;
            CameraPrev.CameraPreview camPrev = new CameraPrev.CameraPreview();
            camPrev.HorizontalOptions = LayoutOptions.FillAndExpand;
            camPrev.VerticalOptions = LayoutOptions.FillAndExpand;
            PreviewGrid.Children.Insert(0, camPrev);
        }



        private void OnPictureFinished()
        {
            //PreviewImage.Source = ImageSource.FromStream(()=>new MemoryStream(takenImage));

        }

        bool send = true;
        bool play = true;
        public bool SendFrameFunc()
        {
            if (takenImage != null)
                SendFrame();
            if (play)
            {
                //GetNextFrame(this, EventArgs.Empty);

            }
            return true;
        }

        int count = 1;
        public void SendFrame()
        {
            ActuallySendFrame();
        }

        bool sending = false;
        int imageSizeInBytes = 5000;
        double speed;
        DateTime before;
        public DateTime lastFrameSent = DateTime.Now;
        Queue<PacketBuffer> sendQueue = new Queue<PacketBuffer>();
        public async void ActuallySendFrame()
        {
            if (DateTime.Now - lastFrameSent > TimeSpan.FromSeconds(.085))//!sending)
            {
                lastFrameSent = DateTime.Now;
                PacketBuffer buffer = new PacketBuffer();
                buffer.WriteInteger((int)ClientPackets.VideoFrame);
                //before = DateTime.Now;
                if (takenImage != null)
                {
                    int f = frame;
                    frame++;
                    int quality;
                    double resizeFactor;
                    int maxBytes = Math.Max(1000, imageSizeInBytes);
                    if (maxBytes <= 1000)
                    {
                        quality = 15;
                        resizeFactor = .3;
                    }
                    else if (maxBytes <= 2500)
                    {
                        quality = 20;
                        resizeFactor = .35;
                    }
                    else if (maxBytes <= 5000)
                    {
                        quality = 30;
                        resizeFactor = .4;
                    }
                    else if (maxBytes <= 10000)
                    {
                        quality = 40;
                        resizeFactor = .5;
                    }
                    else if (maxBytes <= 15000)
                    {
                        quality = 50;
                        resizeFactor = .55;
                    }
                    else if (maxBytes <= 20000)
                    {
                        quality = 60;
                        resizeFactor = .65;
                    }
                    else if (maxBytes <= 50000)
                    {
                        quality = 70;
                        resizeFactor = .7;
                    }
                    else
                    {
                        quality = 100;
                        resizeFactor = 1;
                    }
                    buffer.WriteJpeg(await ImageResizerForms.ResizeImage(takenImage, .5, 50, 25000), f);

                }
                else
                {
                    return;
                }
                double timeToCompress = (DateTime.Now - before).TotalMilliseconds;
                Debug.WriteLine("Time to compress:" + timeToCompress + "ms");
                Debug.WriteLine("Packet size: " + buffer.Length() / 1000 + "KB");
                Debug.WriteLine(Plugin.DeviceOrientation.CrossDeviceOrientation.Current.CurrentOrientation);
                before = DateTime.Now;
                DateTime b = DateTime.Now;
                sending = true;

                await c.Send(buffer.ToArray());
                sending = false;
                //CheckSendQueue();
                speed = buffer.Length() / (DateTime.Now - b).TotalSeconds;
                if (speed / buffer.Length() < 1.5 * (1 / .085) || (timeToCompress > 125 && buffer.Length() > 10000))
                {
                    imageSizeInBytes -= 1000;
                    imageSizeInBytes = Math.Max(imageSizeInBytes, 5000);
                }
                else if ((speed / imageSizeInBytes > 2 * (1 / .085) && timeToCompress < 100) || (timeToCompress > 125 && buffer.Length() < 7500))
                {
                    imageSizeInBytes += 1000;
                    imageSizeInBytes = Math.Min(imageSizeInBytes, 35000);
                }
                Debug.WriteLine("Speed: " + speed / 1000000);
                sending = false;
                buffer.Dispose();


            }
            count++;
        }

        public async void CheckSendQueue()
        {
            if (sendQueue.Count > 0)
            {
                await c.Send(sendQueue.Dequeue().ToArray());
            }
        }

        int index = 1;
        public void GetNextFrame(object sender, EventArgs e)
        {
            ChangeCamera(!CameraIsFront);
            index++;
            index = Math.Min(85, index);
            //PreviewImage.Source = ImageSource.FromStream(() => App.assetOpener.GetFrame(index));

            if (index == 85)
            {
                index = 0;
            }
            //await Task.Delay(25);
            //BackgroundImage.Source = PreviewImage.Source;
        }

        public void GetPrevFrame(object sender, EventArgs e)
        {
            index--;
            index = Math.Max(1, index);
            //PreviewImage.Source = ImageSource.FromStream(() => App.assetOpener.GetFrame(index));
        }

        public void PlayToggled(object sender, EventArgs e)
        {
            play = ((Xamarin.Forms.Switch)sender).IsToggled;
        }

        public SKBitmap CameraPreview { get { return cameraPreview; } set { cameraPreview = value; OnCameraPreviewChange(); } }

        public bool StartingCameraIsFront { get; private set; }

        private SKBitmap cameraPreview;

        public void OnCameraPreviewChange()
        {
            Device.BeginInvokeOnMainThread(() => PreviewCanvas.InvalidateSurface());
            ActuallySendFrame();
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
                    //canvas.DrawBitmap(CameraPreview, SKRect.Create(info.Width, info.Height), paint);

                    int rotation = 0;
                    int offset = 0;
                    int orientation = (int)Plugin.DeviceOrientation.CrossDeviceOrientation.Current.CurrentOrientation;
                    if (!CameraIsFront)//StartingCameraIsFront && !CameraIsFront || (!StartingCameraIsFront && CameraIsFront))
                    {
                        if (orientation == 2)
                        {
                            orientation = 8;
                        }
                        else if (orientation == 8)
                        {
                            orientation = 2;
                        }
                    }
                    if (orientation == 0)
                    {
                        rotation = 270;
                        offset = -CameraPreview.Width;
                    }
                    else if (orientation == 2)
                    {
                        rotation = 270;
                        offset = -CameraPreview.Width;
                    }
                    else if (orientation == 4)
                    {
                        rotation = 180;
                        offset = -CameraPreview.Height;
                    }
                    else if (orientation == 8)
                    {
                        rotation = 90;
                        offset = CameraPreview.Width;
                    }
                    //var rotated = new SKBitmap(info.Height, info.Width);
                    //canvas.Translate(offset, 0);
                    canvas.RotateDegrees(rotation, info.Width / 2, info.Height / 2);
                    if (rotation == 90 || rotation == 270)
                    {
                        float scale = Math.Min((float)info.Width / CameraPreview.Height,
                               (float)info.Height / CameraPreview.Width);
                        float x = (info.Width - scale * CameraPreview.Height) / 2;
                        float y = (info.Height - scale * CameraPreview.Width) / 2;
                        SKRect destRect = new SKRect(x, y, x + scale * CameraPreview.Width,
                                                           y + scale * CameraPreview.Height);
                        //canvas.DrawBitmap(CameraPreview, SKRect.Create(info.Height, info.Width), paint);
                        canvas.Translate(destRect.Height / 2f - destRect.Width / 2f, destRect.Width / 2f - destRect.Height / 2f);
                        canvas.DrawBitmap(CameraPreview, destRect, paint);
                    }
                    else
                    {
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

        public SKBitmap CameraClientPreview { get { return cameraClientPreview; } set { cameraClientPreview = value; OnCameraClientPreviewChange(); } }
        private SKBitmap cameraClientPreview;

        public void OnCameraClientPreviewChange()
        {
            Debug.WriteLine("frame updated");
            Device.BeginInvokeOnMainThread(() => canvasView.InvalidateSurface());
        }

        void OnCanvasViewPaintSurfaceClient(object sender, SKPaintSurfaceEventArgs args)
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
                    float scale = Math.Min((float)info.Width / CameraClientPreview.Width,
                               (float)info.Height / CameraClientPreview.Height);
                    float x = (info.Width - scale * CameraClientPreview.Width) / 2;
                    float y = (info.Height - scale * CameraClientPreview.Height) / 2;
                    SKRect destRect = new SKRect(x, y, x + scale * CameraClientPreview.Width,
                                                       y + scale * CameraClientPreview.Height);
                    canvas.DrawBitmap(CameraClientPreview,
                        destRect, paint);
                }
            }
        }
    }
}