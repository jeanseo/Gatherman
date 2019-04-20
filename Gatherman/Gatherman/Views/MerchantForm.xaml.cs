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
            EntryName.Text = this.merchant.Name;
            EntryFirstName.Text = this.merchant.FirstName;
            if (this.merchant.Portrait == null)
            {
                Portrait.Source = ImageSource.FromResource("Gatherman.images.default_portrait.png");
            }
            else
            {
                Portrait.Source = new ImageSourceConverter().ConvertFromInvariantString(this.merchant.Portrait) as ImageSource;
            }

        }

        private async void OnValidate(object sender, EventArgs e)
        {
            if (EditForm == true)
            {
                this.merchant.Name = EntryName.Text;
                this.merchant.FirstName = EntryFirstName.Text;
                this.merchant.Portrait = portraitFileLocation;
                await _connection.UpdateAsync(this.merchant);
            }
            else
            {
                var merchant = new Merchant { Name = EntryName.Text, FirstName = EntryFirstName.Text, Portrait = portraitFileLocation };
            
                await _connection.InsertAsync(merchant);
            }
            
            //_Merchants.Add(merchant);
            await Navigation.PopAsync();
        }
        private async void OnCancel(object sender, EventArgs e)
        {

        }

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

    }
}