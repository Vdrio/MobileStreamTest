using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobileStreamTesting.ImageCorrection
{
    public interface IImageResizer
    {
        Task<byte[]> ResizeImage(byte[] imageData, float width, float height, int quality = 100);
        Task<byte[]> ResizeImage(byte[] imageData, double resizeFactor, int quality = 100, int maxBytes = 45000);
        Task<byte[]> ResizeImageToByteSize(byte[] imageData, int maxBytes);
    }   
}
