using Android.Content;
using Android.Graphics;
using MobileStreamTesting.CameraPrev;
using MobileStreamTesting.Droid.CameraPrev;
using MobileStreamTesting.ImageCorrection;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using CameraPreview = MobileStreamTesting.CameraPrev.CameraPreview;

[assembly: ExportRenderer(typeof(CameraPreview), typeof(CameraViewServiceRenderer))]
namespace MobileStreamTesting.Droid.CameraPrev
{
    public class CameraViewServiceRenderer : ViewRenderer<CameraPreview, CameraDroid>
    {
        private CameraDroid _camera;
        private CameraPreview _currentElement;
        private readonly Context _context;

        public static CameraViewServiceRenderer renderer;
        public byte[] takenPhoto;

        public CameraViewServiceRenderer(Context context) : base(context)
        {
            renderer = this;
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<MobileStreamTesting.CameraPrev.CameraPreview> e)
        {
            base.OnElementChanged(e);

            System.Diagnostics.Debug.WriteLine("/////////////////Made new camera");
            _camera = new CameraDroid(Context);

            SetNativeControl(_camera);

            if (e.NewElement != null && _camera != null)
            {
                e.NewElement.CameraClick = new Command(() => TakePicture());
                _currentElement = e.NewElement;
                _camera.Photo += OnPhoto;
            }
        }

        public void TakePicture()
        {
            _camera.LockFocus();
        }

        private void OnPhoto(object sender, byte[] imgSource)
        {
            //Here you have the image byte data to do whatever you want 
            System.Diagnostics.Debug.WriteLine("image byte array set");
            takenPhoto = imgSource;
            HostPage.takenImage = takenPhoto;

            Device.BeginInvokeOnMainThread(() =>
            {
                _currentElement?.PictureTaken();
            });
        }

        protected override void Dispose(bool disposing)
        {
            _camera.Photo -= OnPhoto;

            base.Dispose(disposing);
        }
    }
}