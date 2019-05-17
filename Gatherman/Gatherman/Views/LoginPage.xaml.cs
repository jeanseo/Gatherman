using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gatherman.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gatherman.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
        public User user;

        public LoginPage ()
		{
			InitializeComponent ();
            user = new User();

            userNameEntry.Completed += (sender, args) => { passwordEntry.Focus(); };
            //Lorsque OK après le mot de passe, on s'identifie
            //passwordEntry.Completed += (sender, args) => { ViewModel.AuthenticateCommand.Execute(null); };
            
        }


        //Bloque l'appui sur précédent
        //protected override bool OnBackButtonPressed() => true;

        private async void OnLogin(object sender, EventArgs e)
        {
            user.username = userNameEntry.Text;
            user.password = passwordEntry.Text;
            var isAuthenticated = await user.isAuthenticated();
            if (isAuthenticated)
            {
                Application.Current.Properties[Constants.KEY_CONNECTED] = true;
                await Application.Current.SavePropertiesAsync();

                await Navigation.PushModalAsync(App.mainPage);
                //Device.BeginInvokeOnMainThread(() => App.Current.MainPage = MainPage);
            }
            else
            {
                await DisplayAlert("Erreur", "le login ou le mot de passe est incorrect", "OK");
            }
            
            
        }

	}
    
}