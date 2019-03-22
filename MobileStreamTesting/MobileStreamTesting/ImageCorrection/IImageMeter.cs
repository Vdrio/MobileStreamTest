using System;
using System.Collections.Generic;
using System.Text;

namespace MobileStreamTesting.ImageCorrection
{
    public interface IImageMeter
    {
        Size GetImageSize(byte[] image);
    }
    
    public class Size
    {
        public int width;
        public int height;
        public Size(int Width, int Height)
        {
            width = Width;
            height = Height;
        }
    }
}
