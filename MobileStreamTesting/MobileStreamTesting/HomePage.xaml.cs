using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileStreamTesting
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
		public HomePage ()
		{
			InitializeComponent ();
		}

        private void HostClicked(object sender, EventArgs e)
        {
            App.Current.MainPage.Navigation.PushAsync(new HostPage());
        }
        private void ClientClicked(object sender, EventArgs e)
        {
            App.Current.MainPage.Navigation.PushAsync(new ClientPage());
        }
        private void ImageResizeClicked(object sender, EventArgs e)
        {
            App.Current.MainPage.Navigation.PushAsync(new MainPage());
        }
    }
}