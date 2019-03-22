using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
#if __IOS__
using UIKit;
#endif

#if __ANDROID__
using Android.App;
using Android.Graphics;
using Android.Content.Res;
#endif

namespace MobileStreamTesting.ImageCorrection
{
    public static class ImageMeter
    {
        public static Size GetImageSize(byte[] image)
        {
#if __IOS__
			UIImage image = UIImage.FromFile(fileName);
			return new Size((double)image.Size.Width, (double)image.Size.Height);
#endif

#if __ANDROID__
			var options = new BitmapFactory.Options {
                InJustDecodeBounds = true
            };

            fileName = fileName.Replace(".png", "").Replace(".jpg", "");
            var resField = typeof(Resource.Drawable).GetField(fileName);
            var resID = (int) resField.GetValue(null);

            BitmapFactory.DecodeResource(Forms.Context.Resources, resID, options);
            return new Size((double)options.OutWidth, (double)options.OutHeight);
#endif

            return new Size(0,0);
        }
    }
}
