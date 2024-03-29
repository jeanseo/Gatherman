﻿using Gatherman.Data;
using Gatherman.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using System.Collections.ObjectModel;
using Gatherman.Views;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Gatherman.DataAccess
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DBAccess : ContentPage
	{
        public string personAddIcon { get; } = Constants.PersonAdd;
        public string editIcon { get; } = Constantes.IconFont.edit;
        //Si on est pas déjà connecté, on fait apparaitre la page de connexion
        public Models.User loggedUser;

        //Déclaration de la liaison avec la base de données
        private SQLiteAsyncConnection _connection;
        // Liste (Collection d'objets) qui va apparaitre dans le Xaml
        private ObservableCollection<Merchant> _Merchants;

		public DBAccess (Models.User _user)
		{
            loggedUser = _user;
            // Je crée ma connection avec la base de données
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
           

		}

        //Au chargement de la page
        protected override async void OnAppearing()
        {
            InitializeComponent();
            this.IsBusy = true;
            BindingContext = this;
            await _connection.CreateTableAsync<Merchant>();


            //Si c'est le premier lancement, il faut faire une requête get sur l'api
            var merchantService = new MerchantService();
            //var merchantList = new List<Merchant>();
            
            //On regarde si on doit lancer une initialisation ou une mise à jour des données
            if (!Application.Current.Properties.ContainsKey(Constants.KEY_LASTSYNC) || Application.Current.Properties[Constants.KEY_LASTSYNC] == null)
            {
                if(!Constantes.offline.isOffline)
                    await merchantService.initializeMerchantList(loggedUser);
            }
            else
            {
                if (!Constantes.offline.isOffline)
                    await merchantService.syncMerchant(loggedUser);
            }
            //On charge les données de la base locale
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            var MerchantList = new List<Merchant>();
            MerchantList =await _connection.QueryAsync<Merchant>("SELECT * FROM Merchant WHERE deleted=?", false);
            _Merchants = new ObservableCollection<Merchant>(MerchantList);
            Debug.Write("----------Liste affichée----\n" + JsonConvert.SerializeObject(MerchantList));
            lstVMerchant.ItemsSource = null;
            lstVMerchant.ItemsSource = _Merchants;
            this.IsBusy = false;

            //test de sélection d'un item
            lstVMerchant.ItemSelected += (sender, e) =>
            {
                if (lstVMerchant.SelectedItem != null)
                {
                    Merchant item = lstVMerchant.SelectedItem as Merchant;
                    Navigation.PushAsync(new MerchantForm(item));
                }
                
            };

            lstVMerchant.RefreshCommand = new Command(async () => await RefreshList());
             
            async Task RefreshList()
            {
                this.IsBusy = true;
                Debug.Write("RefreshCommand");
                if (!Constantes.offline.isOffline)
                    await merchantService.syncMerchant(loggedUser);
                _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
                MerchantList = await _connection.QueryAsync<Merchant>("SELECT * FROM Merchant WHERE deleted=?", false);
                _Merchants = null;
                _Merchants = new ObservableCollection<Merchant>(MerchantList);
                lstVMerchant.ItemsSource = null;
                lstVMerchant.ItemsSource = _Merchants;
                lstVMerchant.IsRefreshing = false;
                this.IsBusy = false;

            }

        }

        private async void OnAdd(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MerchantForm() );
        }
        private async void OnDelete(object sender, EventArgs e)
        {
            var b = ((ImageButton)sender);
            var merchantToDelete = b.CommandParameter as Merchant;
            merchantToDelete.deleted = true;
            merchantToDelete.lastUpdated = DateTime.Now;
            await _connection.UpdateAsync(merchantToDelete);
            // Retirer l'objet de la liste view
            _Merchants.Remove(merchantToDelete);
        }
        private async void OnEdit(object sender, EventArgs e)
        {
            var b = ((ImageButton)sender);
            var merchantToEdit = b.CommandParameter as Merchant;
            await Navigation.PushAsync(new MerchantForm(merchantToEdit));
        }


        public Command SwipeLeftCommand => new Command(() =>
        {
            //do something
            DisplayAlert("coucou", "Vous avez cliqué sur un marchand", "OK");
        });
    }
}