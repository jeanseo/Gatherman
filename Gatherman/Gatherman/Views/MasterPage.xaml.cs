using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public MasterPage(Models.User _loggedUser)
        {
            InitializeComponent();
            loggedUser = _loggedUser;
            BindingContext = this;
            labelName.Text = loggedUser.fullName;
            labelEmail.Text = loggedUser.email;
            labelPhoneNumber.Text = loggedUser.phoneNumber;
            portrait.Source = loggedUser.pictureFullPath;

        }

        protected override async void OnAppearing()
        {
            offlineSwitch.IsToggled = Constantes.offline.isOffline;

            var picture = ImageSource.FromResource("Gatherman.images.default_portrait.png");
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            Constantes.offline.isOffline = (bool)e.Value;
            Debug.Write("----VALEUR DE ISOFFLINE----\n" + Constantes.offline.isOffline);
        }

        private async void onLogOff(object sender, EventArgs e)
        {
            await loggedUser.logOff();
            Constantes.offline.isOffline = false;
            Application.Current.MainPage = new LoginPage(loggedUser);
        }
    }
}