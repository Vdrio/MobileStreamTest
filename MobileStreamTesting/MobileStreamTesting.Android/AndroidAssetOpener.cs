using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MobileStreamTesting.Droid
{
    public class AndroidAssetOpener : NativeAssetOpener
    {
        public Stream GetSampleFileStream()
        {
            AssetManager assets = MainActivity.mainActivity.Assets;        
            return assets.Open("testpng.png");
        }

        public Stream GetFrame(int frame)
        {
            AssetManager assets = MainActivity.mainActivity.Assets;
            return assets.Open("CleanDeag " + (frame * 3 - 2).ToString("000") + ".jpg");
        }

    }
}