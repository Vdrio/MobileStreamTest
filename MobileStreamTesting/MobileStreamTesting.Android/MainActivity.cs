using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using FFImageLoading.Forms;
using FFImageLoading.Forms.Platform;
using Plugin.Permissions;
using MobileStreamTesting.Droid.ImageCorrection;

namespace MobileStreamTesting.Droid
{
    [Activity(Label = "MobileStreamTesting", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static MainActivity mainActivity;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            mainActivity = this;
            CachedImageRenderer.Init(true);
            App.InitializeAssetOpener(new AndroidAssetOpener());
            App.InitializeImageMeter(new ImageMeterDroid());
            App.InitializeImageResizer(new ImageResizerDroid());
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            
            
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}