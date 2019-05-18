using Gatherman.Data;
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
            loggedUser = new Models.User();
            if (!Application.Current.Properties.ContainsKey(Constants.KEY_CONNECTED))
            {
                Application.Current.Properties[Constants.KEY_CONNECTED] = false;
                Application.Current.SavePropertiesAsync();
            }

            InitializeComponent();
            if((bool)Application.Current.Properties[Constants.KEY_CONNECTED])
            {
                MainPage = mainPage(loggedUser);
                /*MainPage = new MasterDetailPage()
                {
                    Master = new Gatherman.Views.MasterPage() { Title = "Main Page" },
                    Detail = new NavigationPage(new Gatherman.DataAccess.DBAccess())
                };*/
            }
            else
            {
                    MainPage = new Views.LoginPage(loggedUser);
            }

        }
        
        protected override void OnStart()
        {
            
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
