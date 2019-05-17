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
        

        //Déclaration de la liaison avec la base de données
        private SQLiteAsyncConnection _connection;
        // Liste (Collection d'objets) qui va apparaitre dans le Xaml
        private ObservableCollection<Merchant> _Merchants;

		public DBAccess ()
		{
            // Je crée ma connection avec la base de données
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();

           

		}

        //Au chargement de la page
        protected override async void OnAppearing()
        {
            InitializeComponent();
            
            //Si c'est le premier lancement, il faut faire une requête get sur l'api
            var merchantService = new MerchantService();
            //var merchantList = new List<Merchant>();
            

            if (!Application.Current.Properties.ContainsKey(Constants.KEY_LASTSYNC) || Application.Current.Properties[Constants.KEY_LASTSYNC] == null)
            {
                await merchantService.initializeMerchantList();

            }
            else
            {
                await merchantService.syncMerchant();

            }
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            var MerchantList = new List<Merchant>();
            MerchantList.AddRange(await _connection.QueryAsync<Merchant>("SELECT * FROM Merchant WHERE deleted=?", false));
            _Merchants = new ObservableCollection<Merchant>(MerchantList);
            Debug.Write("----------\n" + JsonConvert.SerializeObject(MerchantList));
            lstVMerchant.ItemsSource = _Merchants;


            //On crée la table Merchant avec un objet Merchant
            //On crée une liste qui va récupérer les informations de la base de données (attente de la connexion)
            //var MerchantList = await _connection.Table<Merchant>().Where(x => x.deleted == false).ToListAsync();
            //var MerchantList = new List<Merchant>();
            //MerchantList.AddRange(await _connection.QueryAsync<Merchant>("SELECT * FROM Merchant WHERE deleted=?", false));

            //            var merchantService = new MerchantService();
            //            await merchantService.syncMerchant();


            


            //test de sélection d'un item
            lstVMerchant.ItemSelected += (sender, e) =>
            {
                if (lstVMerchant.SelectedItem != null)
                {
                    //Merchant item = lstVMerchant.SelectedItem as Merchant;
                    //DisplayAlert(item.FullName, "Vous avez cliqué sur un marchand", "OK");
                    //                lstVMerchant.SelectedItem = null;
                }
                
            };

            lstVMerchant.RefreshCommand = new Command((obj) =>
            {
                Debug.Write("RefreshCommand");
                merchantService.syncMerchant();
                lstVMerchant.ItemsSource = _Merchants;
                lstVMerchant.IsRefreshing = false;
            });

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
            merchantToDelete.lastUpdated = DateTime.UtcNow;
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