using MobileStreamTesting.ImageCorrection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace MobileStreamTesting.UWP.ImageCorrection
{
    public class ImageResizerUWP:IImageResizer
    {
        public async Task<byte[]> ResizeImage(byte[] imageData, float width, float height, int quality = 80)
        {
            byte[] resizedData;
            if (quality > 100) quality = 100;
            else if (quality < 1) quality = 1;

            var propertySet = new Windows.Graphics.Imaging.BitmapPropertySet();
            var qualityValue = new Windows.Graphics.Imaging.BitmapTypedValue(
                quality / 100.0, 
                Windows.Foundation.PropertyType.Single
            );

            propertySet.Add("ImageQuality", qualityValue);

            using (var streamIn = new MemoryStream(imageData))
            {
                using (var imageStream = streamIn.AsRandomAccessStream())
                {
                    var resizedStream = new InMemoryRandomAccessStream();
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, resizedStream, propertySet);
                    encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;
                    encoder.BitmapTransform.ScaledHeight = (uint)height;
                    encoder.BitmapTransform.ScaledWidth = (uint)width;
                    await encoder.FlushAsync();
                    resizedStream.Seek(0);
                    resizedData = new byte[resizedStream.Size];
                    await resizedStream.ReadAsync(resizedData.AsBuffer(), (uint)resizedStream.Size, InputStreamOptions.None);
                }
            }

            return resizedData;
        }

        public async Task<byte[]> ResizeImage(byte[] imageData, double resizeFactor, int quality = 100)
        {
            byte[] resizedData;
            if (quality > 100) quality = 100;
            else if (quality < 1) quality = 1;

            var propertySet = new Windows.Graphics.Imaging.BitmapPropertySet();
            var qualityValue = new Windows.Graphics.Imaging.BitmapTypedValue(
                quality / 100.0,
                Windows.Foundation.PropertyType.Single
            );

            propertySet.Add("ImageQuality", qualityValue);

            using (var streamIn = new MemoryStream(imageData))
            {
                using (var imageStream = streamIn.AsRandomAccessStream())
                {
                    var decoder = await BitmapDecoder.CreateAsync(imageStream);
                    var resizedStream = new InMemoryRandomAccessStream();
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, resizedStream, propertySet);
                    encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;
                    encoder.BitmapTransform.ScaledHeight = Convert.ToUInt32(decoder.PixelHeight * resizeFactor);
                    encoder.BitmapTransform.ScaledWidth = Convert.ToUInt32(decoder.PixelWidth * resizeFactor);
                    await encoder.FlushAsync();
                    resizedStream.Seek(0);
                    resizedData = new byte[resizedStream.Size];
                    await resizedStream.ReadAsync(resizedData.AsBuffer(), (uint)resizedStream.Size, InputStreamOptions.None);
                }
            }

            return resizedData;
        }

        public Task<byte[]> ResizeImageToByteSize(byte[] imageData, int maxBytes)
        {
            throw new NotImplementedException();
        }
    }
}
