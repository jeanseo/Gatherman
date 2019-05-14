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
        public App()
        {
            InitializeComponent();
            
            //MainPage = new NavigationPage(new Gatherman.DataAccess.DBAccess());
            MainPage = new MasterDetailPage()
            {
                Master = new Gatherman.Views.MasterPage() { Title = "Main Page" },
                Detail = new NavigationPage(new Gatherman.DataAccess.DBAccess())
            };

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
