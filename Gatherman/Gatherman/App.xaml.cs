using Gatherman.Data;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Gatherman
{
    public partial class App : Application
    {
        public Models.User loggedUser;

        public static MasterDetailPage mainPage(Models.User loggedUser) {
                return new MasterDetailPage()
                {
                    Master = new Gatherman.Views.MasterPage() { Title = "Main Page" },
                    Detail = new NavigationPage(new Gatherman.DataAccess.DBAccess(loggedUser))
                };
        }


        public App()
        {
            //1ère connexion
            if (!Application.Current.Properties.ContainsKey(Constants.KEY_CONNECTED))
            {
                Application.Current.Properties[Constants.KEY_CONNECTED] = false;
                Application.Current.Properties[Constants.KEY_CREDENTIALS] = null;
                Application.Current.SavePropertiesAsync();
            }
            loggedUser = new Models.User();
            InitializeComponent();
            //Si on est déjà connecté
            MainPage = new Views.LoginPage(loggedUser);

        }
        
        protected async override void OnStart()
        {
            /*
            if ((bool)Application.Current.Properties[Constants.KEY_CONNECTED])
            {
                //TODO Récupérer les identifiants stockés
                string jsonUser = (string)Application.Current.Properties[Constants.KEY_CREDENTIALS];
                Debug.Write("-----DONNEES STOCKEES------\n" + jsonUser);
                loggedUser = JsonConvert.DeserializeObject<Models.User>(jsonUser);
                Debug.Write("-----DONNEES TRAITEES------\n" + loggedUser.id);

                //TODO Tenter de se connecter avec les identifiants stockés
                bool isConnected = await loggedUser.isAuthenticated();

                if (isConnected)
                {
                    MainPage = mainPage(loggedUser);
                }
                else
                {
                    MainPage = new Views.LoginPage(loggedUser);
                }

            }
            //Si on est pas encore connecté
            else
            {
                loggedUser = new Models.User();

                MainPage = new Views.LoginPage(loggedUser);
            }
            */
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
