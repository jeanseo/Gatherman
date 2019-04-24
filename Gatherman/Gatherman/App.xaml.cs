﻿using System;
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
            //MainPage = new Gatherman.MainPage();
            MainPage = new MasterDetailPage()
            {
                Master = new Gatherman.Views.MasterPage() { Title = "Main Page" },
                Detail = new NavigationPage(new Gatherman.DataAccess.DBAccess())
            };

        }

        protected override void OnStart()
        {
            // Handle when your app starts
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
