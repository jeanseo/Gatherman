using Gatherman.Data;
using Gatherman.DataAccess.Model;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SQLite;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
using Rg.Plugins.Popup.Services;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Gatherman.Models;
using System.Collections.ObjectModel;
using Gatherman.ViewModels;

namespace Gatherman.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MerchantForm : ContentPage
	{
        public class MyMessage
        {
            public int Myvalue { get; set; }
        }

        //TODO Binder cette valeur au titre de la page
        private string pageTitle
            {
                get
                {
                if (this.EditForm == false)
                    return "Ajout d'un commerçant";
                else
                    return "Edition d'un commerçant";
                }
            }
        private bool EditForm;
        public Merchant merchant;
        private string pictureFilePath;
        private string pictureFileName;


        //Déclaration de la liaison avec la base de données
        private SQLiteAsyncConnection _connection;
        public MerchantForm ()
		{
            InitializeComponent();
            // Je crée ma connection avec la base de données
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            this.EditForm = false;
            Picture.Source = ImageSource.FromResource("Gatherman.images.default_portrait.png");
        }

        private async Task<List<Market>> GetMarketList()
        {
            var db = _connection.Table<Market>();
            return await db.ToListAsync();
        }

        public MerchantForm(Merchant _merchant)
        {
            InitializeComponent();

            // Je crée ma connection avec la base de données
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            this.merchant = _merchant;
            this.EditForm = true;
            //TODO Charger toutes les données
            EntryName.Text = this.merchant.lastName;
            EntryFirstName.Text = this.merchant.firstName;
            Picture.Source = this.merchant.pictureFullPath;
            

        }

        protected override async void OnAppearing()
        {
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            Debug.Write(await GetMarketList());
            this.BindingContext = new RootModel
            {
                MarketList = await GetMarketList()
            };
            var _MarketList = await GetMarketList();
            Debug.Write(JsonConvert.SerializeObject(_MarketList));
            if (EditForm == true)
            {
                for (int x = 0; x < _MarketList.Count; x++)
                {
                    if (_MarketList[x].id == this.merchant.marketid)
                    {
                        PickerMarket.SelectedIndex = x;
                    }
                }
            }
            MessagingCenter.Subscribe<MyMessage>(this, "PopUpData", (value) =>
            {
                int choice = value.Myvalue;
                Debug.Write("----CHOIX EFFECTUE-------\n" + choice);
                switch (choice)
                {
                    case 1:
                        OnCamera();
                        break;
                    case 2:
                        OnPickPicture();
                        break;
                }
            });
        }

        protected override void OnDisappearing()
        {
            MessagingCenter.Unsubscribe<MyMessage>(this, "PopUpData");
        }

        private async void OnValidate(object sender, EventArgs e)
        {
            if (EditForm == true)
            {
                this.merchant.lastName = EntryName.Text;
                this.merchant.firstName = EntryFirstName.Text;
                this.merchant.email = EntryEmail.Text;
                this.merchant.phone = EntryPhone.Text;
                this.merchant.incoming = Convert.ToInt32(EntryIncoming.Text);
                this.merchant.holidays = Convert.ToInt32(EntryHolidays.Text);
                this.merchant.lastUpdated = DateTime.Now;

                if (this.merchant.pictureFileName != null && pictureFileName != null)
                {
                    //TODO si on change la photo, il faut supprimer l'ancien fichier image
                }
                if (pictureFileName != null)
                {
                    this.merchant.pictureFileName = pictureFileName;
                    this.merchant.pictureLocalPath = pictureFilePath;

                }
                await _connection.UpdateAsync(this.merchant);

            }
            else
            {
                Guid id = Guid.NewGuid();
                Debug.Write(id);
                var merchant = new Merchant {
                    lastName = EntryName.Text,
                    firstName = EntryFirstName.Text,
                    pictureFileName = pictureFileName,
                    pictureLocalPath = pictureFilePath,
                    email = EntryEmail.Text,
                    phone = EntryPhone.Text,
                    incoming = Convert.ToInt32(EntryIncoming.Text),
                    holidays = Convert.ToInt32(EntryHolidays.Text),
                    creationDate = DateTime.Now,
                    lastUpdated = DateTime.Now,
                    deleted = false ,id = id
            };
            
                await _connection.InsertAsync(merchant);

            }
            
            //_Merchants.Add(merchant);
            await Navigation.PopAsync();
        }
        /*private async void OnCancel(object sender, EventArgs e)
        {

        }*/

        private async void OnCamera()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        await DisplayAlert("Autorisation nécessaire", "Autorisation de stockage obligatoire", "OK");
                    }

                    var response = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                    status = response[Permission.Storage];
                }

                if (status == PermissionStatus.Granted)
                {
                    if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                    {
                        // Supply media options for saving our photo after it's taken.
                        var mediaOptions = new StoreCameraMediaOptions
                        {
                            Directory = "Receipts",
                            Name = $"{Guid.NewGuid()}.jpg",
                            PhotoSize = PhotoSize.Medium,
                            CompressionQuality = 80
                        };

                        // Take a photo of the business receipt.
                        var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);
                        if (file != null)
                        {
                            Picture.Source = file.Path;
                            pictureFilePath = Path.GetDirectoryName(file.Path);
                            pictureFileName = Path.GetFileName(file.Path);
                        }
                    }
                }
                else if (status != PermissionStatus.Unknown)
                {
                    //location denied
                }

            }
            catch (Exception ex)
            {

            }
            //TODO Verifier les autorisations
            
        }

        private async void OnPickPicture()
            {
            //TODO Gérer si on annule la prise de vue
            //TODO Changer le nom du fichier pour un nom unique (Guid)
            var mediaOptions = new PickMediaOptions
            {
                    PhotoSize = PhotoSize.Medium,
                    CompressionQuality = 80
                };
                var file = await CrossMedia.Current.PickPhotoAsync(mediaOptions).ConfigureAwait(true);
            if (file != null)
            {
                Picture.Source = file.Path;
                pictureFilePath = Path.GetDirectoryName(file.Path);
                pictureFileName = Path.GetFileName(file.Path);
            }

        }
        private async void onPhotoAdd(object sender, EventArgs e)
        {
            
            await PopupNavigation.PushAsync(new popup.addPicturePopup(),false);
        }

    }
}