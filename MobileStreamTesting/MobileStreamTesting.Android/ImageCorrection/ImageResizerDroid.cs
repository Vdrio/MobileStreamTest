using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileStreamTesting.ImageCorrection;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Android.Graphics;
using System.IO;
using Android.Media;
using Orientation = Android.Media.Orientation;
using MobileStreamTesting.Droid.CameraPrev;

namespace MobileStreamTesting.Droid.ImageCorrection
{
    public class ImageResizerDroid : IImageResizer
    {
        public Task<byte[]> ResizeImage(byte[] imageData, float width, float height, int quality = 100)
        {
            return Task.Run(() =>
            {
                if (quality > 100) quality = 100;
                else if (quality < 1) quality = 1;
                // Load the bitmap
                Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
                var matrix = new Matrix();
                
                // rotates the image if needed.
                switch ((int)Plugin.DeviceOrientation.CrossDeviceOrientation.Current.CurrentOrientation)
                {
                    case 0:
                        matrix.PreRotate(90);
                        break;
                    case 2:
                        matrix.PreRotate(270);
                        break;
                    case 4:
                        matrix.PreRotate(180);
                        break;
                    case 8:
                        matrix.PreRotate(90);
                        break;
                }
                Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);
                resizedImage = Bitmap.CreateBitmap(resizedImage, 0,0, (int)width, (int)height, matrix,false);

                using (MemoryStream ms = new MemoryStream())
                {
                    resizedImage.Compress(Bitmap.CompressFormat.Jpeg, quality, ms);
                    return ms.ToArray();
                }
            });
        }

        public Task<byte[]> ResizeImage(byte[] imageData, double resizeFactor, int quality = 100, int maxBytes = 45000)
        {

            if (quality > 100) quality = 100;
            else if (quality < 1) quality = 1;
            // Load the bitmap
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            var matrix = new Matrix();
            int orientation = (int)Plugin.DeviceOrientation.CrossDeviceOrientation.Current.CurrentOrientation;
            if (!HostPage.hostPage.CameraIsFront)
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
            // rotates the image if needed.
            switch (orientation)
            {
                case 0:
                    matrix.PreRotate(90);
                    break;
                case 2:
                    matrix.PreRotate(270);
                    break;
                case 4:
                    matrix.PreRotate(180);
                    break;
                case 8:
                    matrix.PreRotate(90);
                    break;
            }
            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, Convert.ToInt32(originalImage.Width * resizeFactor), Convert.ToInt32(originalImage.Height * resizeFactor), false);
            resizedImage = Bitmap.CreateBitmap(resizedImage, 0, 0, resizedImage.Width, resizedImage.Height, matrix, false);
            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, quality, ms);
                byte[] img = ms.ToArray();
                if (img.Length < maxBytes)
                {
                    return Task.Run(() => { return img; });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("too big, retrying");
                    return ResizeImage(imageData, resizeFactor * .7, Convert.ToInt32(quality * .7));
                }

            }
        }

        public Task<byte[]> ResizeImageToByteSize(byte[] imageData, int maxBytes)
        {
            return Task.Run(() =>
            {
                int quality;
                double resizeFactor;
                maxBytes = Math.Max(1000, maxBytes);
                if (maxBytes <= 100)
                {
                    quality = 15;
                    resizeFactor = .3;
                }
                else if (maxBytes <= 2500)
                {
                    quality = 30;
                    resizeFactor = .4;
                }
                else if (maxBytes <= 5000)
                {
                    quality = 40;
                    resizeFactor = .5;
                }
                else if (maxBytes <= 10000)
                {
                    quality = 50;
                    resizeFactor = .6;
                }
                else if (maxBytes <= 15000)
                {
                    quality = 60;
                    resizeFactor = .65;
                }
                else if (maxBytes <= 20000)
                {
                    quality = 75;
                    resizeFactor = .75;
                }
                else if (maxBytes <= 50000)
                {
                    quality = 90;
                    resizeFactor = .9;
                }
                else
                {
                    quality = 100;
                    resizeFactor = 1;
                }
                
                if (quality > 100) quality = 100;
                else if (quality < 1) quality = 1;
                // Load the bitmap
                Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
                var matrix = new Matrix();

                // rotates the image if needed.
                switch ((int)Plugin.DeviceOrientation.CrossDeviceOrientation.Current.CurrentOrientation)
                {
                    case 0:
                        matrix.PreRotate(90);
                        break;
                    case 2:
                        matrix.PreRotate(270);
                        break;
                    case 4:
                        matrix.PreRotate(180);
                        break;
                    case 8:
                        matrix.PreRotate(90);
                        break;
                }
                byte[] compressedImage;
                Bitmap resizedImage, scaledImage;
                while (true)
                {
                    scaledImage = Bitmap.CreateScaledBitmap(originalImage, Convert.ToInt32(originalImage.Width * resizeFactor), Convert.ToInt32(originalImage.Height * resizeFactor), false);
                    resizedImage = Bitmap.CreateBitmap(scaledImage, 0, 0, scaledImage.Width, scaledImage.Height, matrix, false);
                    scaledImage.Recycle();
                    scaledImage.Dispose();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        resizedImage.Compress(Bitmap.CompressFormat.Jpeg, quality, ms);
                        compressedImage = ms.ToArray();
                        ms.Dispose();                      
                    }
                    if (compressedImage.Length < maxBytes)
                    {
                        originalImage.Recycle();
                        originalImage.Dispose();
                        resizedImage.Recycle();
                        resizedImage.Dispose();
                        matrix.Dispose();
                        System.GC.Collect();
                        return compressedImage;
                    }
                    else if (quality <= 5 && resizeFactor <= .15)
                    {
                        originalImage.Recycle();
                        originalImage.Dispose();
                        resizedImage.Recycle();
                        resizedImage.Dispose();
                        matrix.Dispose();
                        System.GC.Collect();
                        return compressedImage;
                    }
                    else
                    {
                        resizedImage.Recycle();
                        resizedImage.Dispose();
                        quality -= 5;
                        resizeFactor -= .1;
                        quality = Math.Max(5, quality);
                        resizeFactor = Math.Max(.2, resizeFactor);
                    }
                }
            });
        }
    }
}