using Android.Graphics;
using Android.Media;
using MobileStreamTesting.ImageCorrection;
using SkiaSharp;
using System;
using System.Diagnostics;

namespace MobileStreamTesting.Droid.CameraPrev
{
    public class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        public event EventHandler<byte[]> Photo;

        public void OnImageAvailable(ImageReader reader)
        {
            Image image = null;

            try
            {
                image = reader.AcquireLatestImage();
                var buffer = image.GetPlanes()[0].Buffer;
                var imageData = new byte[buffer.Capacity()];
                buffer.Get(imageData);
                Debug.WriteLine("SettingBitmapImage");
                SetBitmapImage(imageData);
                Photo?.Invoke(this, imageData);
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                image?.Close();
            }
        }

        public static SKSize BitmapSize;
        public static void SetBitmapSize(int width, int height)
        {
            BitmapSize = new SKSize(width, height);
        }

        public async static void SetBitmapImage(byte[] jpeg)
        {
            SKBitmap bitmap = SKBitmap.Decode(jpeg);
            if (HostPage.hostPage != null)
            {
                HostPage.hostPage.CameraPreview = bitmap;
                
            }
        }

        public static SKBitmap GetBitmapFromJpeg(byte[] image)
        {
            SKBitmap bitmap = SKBitmap.Decode(image);
            return bitmap;
        }
    }
}