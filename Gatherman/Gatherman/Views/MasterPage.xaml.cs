﻿using System;
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

        public MasterPage ()
		{
			InitializeComponent ();

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
    }
}