using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace MobileStreamTesting.UWP
{
    class JpegTool
    {
        private async Task<StorageFile> ConvertImageToJpegAsync(StorageFile sourceFile, StorageFile outputFile)
        {
            //you can use WinRTXamlToolkit StorageItemExtensions.GetSizeAsync to get file size (if you already plugged this nuget in)
            var sourceFileProperties = await sourceFile.GetBasicPropertiesAsync();
            var fileSize = sourceFileProperties.Size;
            var imageStream = await sourceFile.OpenReadAsync();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            using (imageStream)
            {
                var decoder = await BitmapDecoder.CreateAsync(imageStream);
                var pixelData = await decoder.GetPixelDataAsync();
                var detachedPixelData = pixelData.DetachPixelData();
                pixelData = null;
                //0.85d
                double jpegImageQuality = .85;
                //since we're using MvvmCross, we're outputing diagnostic info to MvxTrace, you can use System.Diagnostics.Debug.WriteLine instead
                var imageWriteableStream = await outputFile.OpenAsync(FileAccessMode.ReadWrite);
                ulong jpegImageSize = 0;
                using (imageWriteableStream)
                {
                    var propertySet = new BitmapPropertySet();
                    var qualityValue = new BitmapTypedValue(jpegImageQuality, Windows.Foundation.PropertyType.Single);
                    propertySet.Add("ImageQuality", qualityValue);
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, imageWriteableStream, propertySet);
                    //key thing here is to use decoder.OrientedPixelWidth and decoder.OrientedPixelHeight otherwise you will get garbled image on devices on some photos with orientation in metadata
                    encoder.SetPixelData(decoder.BitmapPixelFormat, decoder.BitmapAlphaMode, decoder.OrientedPixelWidth, decoder.OrientedPixelHeight, decoder.DpiX, decoder.DpiY, detachedPixelData);
                    await encoder.FlushAsync();
                    
                    jpegImageSize = imageWriteableStream.Size;
                }
            }
            stopwatch.Stop();
            return outputFile;
        }
    }
}
