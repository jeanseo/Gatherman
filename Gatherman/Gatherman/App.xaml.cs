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
        public static MasterDetailPage mainPage {
            get
            {
                return new MasterDetailPage()
                {
                    Master = new Gatherman.Views.MasterPage() { Title = "Main Page" },
                    Detail = new NavigationPage(new Gatherman.DataAccess.DBAccess())
                };
            }
        }


        public App()
        {
            if (!Application.Current.Properties.ContainsKey(Constants.KEY_CONNECTED))
            {
                Application.Current.Properties[Constants.KEY_CONNECTED] = false;
                Application.Current.SavePropertiesAsync();
            }

            InitializeComponent();
            if((bool)Application.Current.Properties[Constants.KEY_CONNECTED])
            {
                MainPage = mainPage;
                /*MainPage = new MasterDetailPage()
                {
                    Master = new Gatherman.Views.MasterPage() { Title = "Main Page" },
                    Detail = new NavigationPage(new Gatherman.DataAccess.DBAccess())
                };*/
            }
            else
            {
                MainPage = new Views.LoginPage();
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
