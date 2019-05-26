using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Gatherman.Views.MerchantForm;

namespace Gatherman.Views.popup
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class addPicturePopup : PopupPage
    {
		public addPicturePopup ()
		{
			InitializeComponent ();
		}

        public async void TakePicture(object sender, EventArgs e)
        {
            Debug.Write("TAKE PICTURE");
            MessagingCenter.Send(new MyMessage() { Myvalue = 1 }, "PopUpData");
            await PopupNavigation.PopAsync(false);
        }

        public async void PickPicture(object sender, EventArgs e)
        {
            Debug.Write("PICK PICTURE");
            MessagingCenter.Send(new MyMessage() { Myvalue = 2 }, "PopUpData");
            await PopupNavigation.PopAsync(false);
        }
    }
}