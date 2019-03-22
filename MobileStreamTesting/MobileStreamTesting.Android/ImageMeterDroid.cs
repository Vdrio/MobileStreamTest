using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileStreamTesting.ImageCorrection;

namespace MobileStreamTesting.Droid
{
    class ImageMeterDroid : IImageMeter
    {
        public Size GetImageSize(byte[] image)
        {
            if (image != null)
            {
                BitmapFactory.Options options = new BitmapFactory.Options()
                {
                    InJustDecodeBounds = true
                };
                Bitmap bitmap = BitmapFactory.DecodeStream(new MemoryStream(image), new Rect(1, 1, 1, 1), options);
                return new Size(options.OutWidth, options.OutHeight);
            }
            return new Size(1, 1);
        }
    }
}