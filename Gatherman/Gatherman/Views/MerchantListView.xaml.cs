using Gatherman.Data;
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
using Plugin.Media;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Gatherman.DataAccess
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DBAccess : ContentPage
	{
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
            await _connection.CreateTableAsync<Merchant>();


            //Si c'est le premier lancement, il faut faire une requête get sur l'api
            var merchantService = new MerchantService();
            //var merchantList = new List<Merchant>();
            
            //On regarde si on doit lancer une initialisation ou une mise à jour des données
            if (!Application.Current.Properties.ContainsKey(Constants.KEY_LASTSYNC) || Application.Current.Properties[Constants.KEY_LASTSYNC] == null)
            {
                if(!Constants.isOffline)
                    await merchantService.initializeMerchantList(loggedUser);
            }
            else
            {
                if (!Constants.isOffline)
                    await merchantService.syncMerchant(loggedUser);
            }
            //On charge les données de la base locale
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            var MerchantList = new List<Merchant>();
            MerchantList.AddRange(await _connection.QueryAsync<Merchant>("SELECT * FROM Merchant WHERE deleted=?", false));
            _Merchants = new ObservableCollection<Merchant>(MerchantList);
            Debug.Write("----------\n" + JsonConvert.SerializeObject(MerchantList));
            lstVMerchant.ItemsSource = null;
            lstVMerchant.ItemsSource = _Merchants;

            //test de sélection d'un item
            lstVMerchant.ItemSelected += (sender, e) =>
            {
                if (lstVMerchant.SelectedItem != null)
                {
                    Merchant item = lstVMerchant.SelectedItem as Merchant;
                    //DisplayAlert(item.FullName, "Vous avez cliqué sur un marchand", "OK");
                    //                lstVMerchant.SelectedItem = null;
                    Debug.Write("\n\nVous averz cliqué sur " + item.FullName);
                }
                
            };

            lstVMerchant.RefreshCommand = new Command(async () => await RefreshList());
             
            async Task RefreshList()
            {
                Debug.Write("RefreshCommand");
                if (!Constants.isOffline)
                    await merchantService.syncMerchant(loggedUser);
                _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
                MerchantList.AddRange(await _connection.QueryAsync<Merchant>("SELECT * FROM Merchant WHERE deleted=?", false));
                lstVMerchant.ItemsSource = null;
                lstVMerchant.ItemsSource = _Merchants;
                lstVMerchant.IsRefreshing = false;
            }

        }

        private async void OnAdd(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MerchantForm() );
        }
        private async void OnDelete(object sender, EventArgs e)
        {
            var b = ((Button)sender);
            var merchantToDelete = b.CommandParameter as Merchant;
            merchantToDelete.deleted = true;
            merchantToDelete.lastUpdated = DateTime.Now;
            await _connection.UpdateAsync(merchantToDelete);
            // Retirer l'objet de la liste view
            _Merchants.Remove(merchantToDelete);
        }
        private async void OnEdit(object sender, EventArgs e)
        {
            var b = ((Button)sender);
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