using MobileStreamTesting.CameraPrev;
using MobileStreamTesting.ImageCorrection;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MobileStreamTesting
{
    public partial class App : Application
    {
        public static NativeAssetOpener assetOpener;
        public static IImageMeter imageMeter;
        public static IImageResizer imageResizer;
        public static ICameraPreview cameraPreview;

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new HomePage());
        }

        public static void InitializeAssetOpener(NativeAssetOpener opener)
        {
            assetOpener = opener;
        }

        public static void InitializeImageMeter(IImageMeter meter)
        {
            imageMeter = meter;
        }

        public static void InitializeImageResizer(IImageResizer resizer)
        {
            imageResizer = resizer;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
