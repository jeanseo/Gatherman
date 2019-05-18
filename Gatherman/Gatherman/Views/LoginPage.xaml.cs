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

            InitializeComponent ();
            userNameEntry.Completed += (sender, args) => { passwordEntry.Focus(); };
            //Lorsque OK après le mot de passe, on s'identifie
            //passwordEntry.Completed += (sender, args) => { ViewModel.AuthenticateCommand.Execute(null); };
            
        }


        //Bloque l'appui sur précédent
        //protected override bool OnBackButtonPressed() => true;

        private async void OnLogin(object sender, EventArgs e)
        {
            loggedUser.username = userNameEntry.Text;
            loggedUser.password = passwordEntry.Text;
            var authenticatedOK = await isAuthenticated();
            if (authenticatedOK)
            {
                Debug.Write(loggedUser.id);
                Application.Current.Properties[Constants.KEY_CONNECTED] = true;
                await Application.Current.SavePropertiesAsync();

                await Navigation.PushModalAsync(App.mainPage(loggedUser));
                //Device.BeginInvokeOnMainThread(() => App.Current.MainPage = MainPage);
            }
            else
            {
                await DisplayAlert("Erreur", "le login ou le mot de passe est incorrect", "OK");
            }
            
            
        }
        public async Task<bool> isAuthenticated()
        {
            HttpResponseMessage response = null;
            //LoginAPIPostObject body = new LoginAPIPostObject { username = username, password = password };
            //User loggedUser = new User { username = username, password = password };
            try
            {


                using (var client = new HttpClient())
                {
                    //Push vers l'API
                    Debug.Write(loggedUser.username);
                    string content = JsonConvert.SerializeObject(loggedUser);
                    Debug.Write("--------Requête JSON-------" + content);

                    response = await client.PostAsync("http://jean-surface:3000/api/Users/login", new StringContent(content, Encoding.UTF8, "application/json"));
                }
                // Handle success
                string responseBody = await response.Content.ReadAsStringAsync();
                loggedUser = JsonConvert.DeserializeObject<User>(responseBody);
                Debug.Write("--------Réponse requête-------" + loggedUser.id);
            }
            catch (Exception ex)
            {
                Debug.Write("--------ERREUR---------\n" + ex);
                return false;

            }

            return true;
        }
    }
    
}