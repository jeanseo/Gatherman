using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gatherman.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MasterPage : ContentPage
	{
        public Models.User loggedUser;

        public MasterPage ()
		{
			InitializeComponent ();

		}

        protected override async void OnAppearing()
        {
            var picture = ImageSource.FromResource("Gatherman.images.default_portrait.png");
        }
    }
}