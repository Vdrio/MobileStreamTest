using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using MobileStreamTesting.ImageCorrection;
using UIKit;

namespace MobileStreamTesting.iOS.ImageCorrection
{
    class ImageResizerIOS:IImageResizer
    {
        public Task<byte[]> ResizeImage(byte[] imageData, float width, float height, int quality = 80)
        {
            if (quality > 100) quality = 100;
            else if (quality < 1) quality = 1;
            nfloat nQuality = new nfloat(Convert.ToDouble(quality) / 100.0);

            UIImage originalImage = ImageFromByteArray(imageData);
            UIImageOrientation orientation = originalImage.Orientation;

            //create a 24bit RGB image
            using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
                                                 (int)width, (int)height, 8,
                                                 4 * (int)width, CGColorSpace.CreateDeviceRGB(),
                                                 CGImageAlphaInfo.PremultipliedFirst))
            {

                RectangleF imageRect = new RectangleF(0, 0, width, height);

                // draw the image
                context.DrawImage(imageRect, originalImage.CGImage);

                UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, orientation);

                // save the image as a jpeg
                return Task.Run(()=> { return resizedImage.AsJPEG(nQuality).ToArray(); });
            }
        }

        public Task<byte[]> ResizeImage(byte[] imageData, double resizeFactor, int quality = 80)
        {
            return Task.Run(() =>
            {
                if (quality > 100) quality = 100;
                else if (quality < 1) quality = 1;
                nfloat nQuality = new nfloat(Convert.ToDouble(quality) / 100.0);

                UIImage originalImage = ImageFromByteArray(imageData);
                UIImageOrientation orientation = originalImage.Orientation;


                //create a 24bit RGB image
                using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero
                                                     , Convert.ToInt32(resizeFactor * originalImage.Size.Width),
                                                     Convert.ToInt32(resizeFactor * originalImage.Size.Height), 8,
                                                     4 * Convert.ToInt32(resizeFactor * originalImage.Size.Width), CGColorSpace.CreateDeviceRGB(),
                                                     CGImageAlphaInfo.PremultipliedFirst))
                {

                    RectangleF imageRect = new RectangleF(0, 0, Convert.ToInt32(resizeFactor * originalImage.Size.Width), Convert.ToInt32(resizeFactor * originalImage.Size.Height));

                    // draw the image
                    context.DrawImage(imageRect, originalImage.CGImage);

                    UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, orientation);

                    // save the image as a jpeg
                    return resizedImage.AsJPEG(nQuality).ToArray();
                }
            });
        }

        public static UIKit.UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            UIKit.UIImage image;
            try
            {
                image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
            }
            catch (Exception e)
            {
                Console.WriteLine("Image load failed: " + e.Message);
                return null;
            }
            return image;
        }

        public Task<byte[]> ResizeImageToByteSize(byte[] imageData, int maxBytes)
        {
            throw new NotImplementedException();
        }
    }
}