using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace MobileStreamTesting.UWP
{
    public class UWPAssetOpener
    {
        WriteableBitmap myBitmap;
        public UWPAssetOpener()
        {
            using (MemoryStream streamOut = new MemoryStream())
            {
                StorageFile file;
                //BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, file.OpenAsync)
                
            }
        }
    }
}
