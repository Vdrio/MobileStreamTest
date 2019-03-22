using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using MobileStreamTesting.iOS.ImageCorrection;
using UIKit;

namespace MobileStreamTesting.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            App.InitializeImageResizer(new ImageResizerIOS());
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}
