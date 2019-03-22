using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileStreamTesting.ImageCorrection
{
    public class ImageResizer
    {
        public static async Task<byte[]> ResizeImageToQuality(CachedImage source, int quality)
        {
            byte[] image = await source.GetImageAsJpgAsync(50, 640, 360);
            double speed = quality * 100000 / 8;
            Debug.WriteLine(speed / image.Length);
            return image;
        }

        public static async Task<byte[]> ResizeImageToQuality(CachedImage source, int jpgQuality, double width = 0, double height = 0)
        {
            if (width == 0 || height == 0)
            {
                width = source.Width;
                height = source.Height;
            }
            byte[] image = await source.GetImageAsJpgAsync(jpgQuality, Convert.ToInt32(width), Convert.ToInt32(height));
            double speed = jpgQuality * 100000 / 8;
            //Debug.WriteLine(speed / image.Length);
            Debug.WriteLine("Image size: " + (image.Length / 1000).ToString() + " KB");
            return image;
        }
        public static async Task<byte[]> ResizeImageToQuality(CachedImage source, int jpgQuality = 50, double resolutionFactor = 1)
        {
            int width = 0, height = 0;
            double ratio = 16/9;
            if (width == 0 || height == 0)
            {
                Size size = App.imageMeter.GetImageSize(await source.GetImageAsJpgAsync(100));
                width = size.width;
                height = size.height;
                ratio = width / height;
            }
            width = Convert.ToInt32(resolutionFactor * width);
            height = Convert.ToInt32(resolutionFactor * height);
            if (width < 240 * ratio|| height < 240)
            {
                height = 240;
                width = Convert.ToInt32(240 * ratio);
            }
            Debug.WriteLine(resolutionFactor);
            byte[] image = await source.GetImageAsJpgAsync(jpgQuality, width, height);
            double speed = jpgQuality * 100000 / 8;
            //Debug.WriteLine("fps: " + speed / image.Length);
            if (image != null)
            {
                Debug.WriteLine("Image size at quality " + jpgQuality + ": " + (image.Length / 1000).ToString() + " KB");
            }
            return image;
        }
    }
}
