using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Gatherman.Models;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gatherman.Views
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
        

        public Models.User loggedUser;

        public LoginPage (Models.User _loggedUser)
		{
            loggedUser = _loggedUser;
            //Si on a déjà été connecté
            
            //Lorsque OK après le mot de passe, on s'identifie
            //passwordEntry.Completed += (sender, args) => { ViewModel.AuthenticateCommand.Execute(null); };
            
        }

        protected override async void OnAppearing()
        {
            if (loggedUser.username != null && loggedUser.password != null)
            {
                //On tente de se reconnecter avec les memes identifiants
                var authenticatedResult = await loggedUser.isAuthenticated();
                if (authenticatedResult == 200)
                {
                    Application.Current.MainPage = App.mainPage(loggedUser);
                }
            }
            InitializeComponent();

            userNameEntry.Completed += (sender, args) => { passwordEntry.Focus(); };

        }


        //Bloque l'appui sur précédent
        //protected override bool OnBackButtonPressed() => true;

        private async void OnLogin(object sender, EventArgs e)
        {
            loggedUser.username = userNameEntry.Text;
            loggedUser.password = passwordEntry.Text;
            var authenticatedResult = await loggedUser.isAuthenticated();
            if (authenticatedResult == 200)
            {
                Debug.Write(loggedUser.id);
                Application.Current.Properties[Constants.KEY_CONNECTED] = true;
                //await loggedUser.SaveUser();
                string userToSave = JsonConvert.SerializeObject(loggedUser);
                Application.Current.Properties[Constants.KEY_CREDENTIALS] = userToSave;
                await Application.Current.SavePropertiesAsync();
                Debug.Write("----------------\n" + Application.Current.Properties[Constants.KEY_CREDENTIALS]);
                //await Navigation.PushModalAsync(App.mainPage(loggedUser));
                //Device.BeginInvokeOnMainThread(() => App.Current.MainPage = MainPage);
                Application.Current.MainPage = App.mainPage(loggedUser);
            }
            else
            {
                await DisplayAlert("Erreur", authenticatedResult.ToString(), "OK");
            }
            
            
        }
    }
    
}