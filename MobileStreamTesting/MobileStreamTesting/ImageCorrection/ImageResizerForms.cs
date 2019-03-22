using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobileStreamTesting.ImageCorrection
{
    public static class ImageResizerForms
    {
        public static Task<byte[]> ResizeImage(byte[] imageData, float width, float height, int quality = 100)
        {
            return App.imageResizer.ResizeImage(imageData, width, height, quality);
        }

        public static Task<byte[]> ResizeImage(byte[] imageData, double resizeFactor, int quality = 100, int maxBytes = 45000)
        {
            return App.imageResizer.ResizeImage(imageData, resizeFactor, quality, maxBytes);
        }
        public static Task<byte[]> ResizeImageToByteSize(byte[] imageData, int maxBytes)
        {
            return App.imageResizer.ResizeImageToByteSize(imageData, maxBytes);
        }
    }
}
