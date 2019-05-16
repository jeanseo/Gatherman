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
namespace Gatherman.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MerchantForm : ContentPage
	{

        private bool EditForm;
        public Merchant merchant;
        private string portraitFileLocation;


        //Déclaration de la liaison avec la base de données
        private SQLiteAsyncConnection _connection;
        public MerchantForm ()
		{

            InitializeComponent();
            // Je crée ma connection avec la base de données
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            this.EditForm = false;
            Portrait.Source = ImageSource.FromResource("Gatherman.images.default_portrait.png");

        }

        public MerchantForm(Merchant _merchant)
        {
            InitializeComponent();

            // Je crée ma connection avec la base de données
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            this.merchant = _merchant;
            this.EditForm = true;
            EntryName.Text = this.merchant.lastName;
            EntryFirstName.Text = this.merchant.firstName;
            if (this.merchant.picturePath == null)
            {
                Portrait.Source = ImageSource.FromResource("Gatherman.images.default_portrait.png");
            }
            else
            {
                Portrait.Source = new ImageSourceConverter().ConvertFromInvariantString(this.merchant.picturePath) as ImageSource;
                picturePath.Text = this.merchant.picturePath;
            }

        }

        private async void OnValidate(object sender, EventArgs e)
        {
            if (EditForm == true)
            {
                this.merchant.lastName = EntryName.Text;
                this.merchant.firstName = EntryFirstName.Text;
                this.merchant.lastUpdated = DateTime.UtcNow;

                if (this.merchant.picturePath != null && portraitFileLocation != null)
                {
                    //TODO si on change la photo, il faut supprimer l'ancien fichier image
                    this.merchant.picturePath = portraitFileLocation;
                }
                await _connection.UpdateAsync(this.merchant);
                //var merchantService = new MerchantService();
                //await merchantService.syncMerchant();


            }
            else
            {
                Guid id = Guid.NewGuid();
                Debug.Write(id);
                var merchant = new Merchant { lastName = EntryName.Text, firstName = EntryFirstName.Text, picturePath = portraitFileLocation, creationDate = DateTime.UtcNow, lastUpdated = DateTime.UtcNow, deleted = false ,id = id
            };
            
                await _connection.InsertAsync(merchant);
                //var merchantService = new MerchantService();
                //await merchantService.syncMerchant();

            }
            
            //_Merchants.Add(merchant);
            await Navigation.PopAsync();
        }
        /*private async void OnCancel(object sender, EventArgs e)
        {

        }*/

        private async void OnCamera(object sender, EventArgs e)
        {
            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                // Supply media options for saving our photo after it's taken.
                var mediaOptions = new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Receipts",
                    Name = $"{DateTime.UtcNow}.jpg",
                    PhotoSize = PhotoSize.Medium,
                    CompressionQuality = 80
                };

                // Take a photo of the business receipt.
                var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);
                Portrait.Source = file.Path;
                portraitFileLocation = Path.GetDirectoryName(file.Path) + "/" + Path.GetFileName(file.Path);
            }
        }

        private async void OnPickPicture(object sender, EventArgs e)
            {
                var mediaOptions = new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                    CompressionQuality = 80
                };
                var file = await CrossMedia.Current.PickPhotoAsync(mediaOptions).ConfigureAwait(true);
                Portrait.Source = file.Path;
                portraitFileLocation = Path.GetDirectoryName(file.Path) + "/" + Path.GetFileName(file.Path);

    }
    }
}