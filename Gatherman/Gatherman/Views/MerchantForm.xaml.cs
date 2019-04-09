using Gatherman.Data;
using Gatherman.DataAccess.Model;
using SQLite;
using System;
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


        //Déclaration de la liaison avec la base de données
        private SQLiteAsyncConnection _connection;
        public MerchantForm ()
		{
			InitializeComponent ();
            // Je crée ma connection avec la base de données
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            this.EditForm = false;
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

        }

        private async void OnValidate(object sender, EventArgs e)
        {
            if (EditForm == true)
            {
                this.merchant.Name = EntryName.Text;
                this.merchant.FirstName = EntryFirstName.Text;
                await _connection.UpdateAsync(this.merchant);
            }
            else
            {
                var merchant = new Merchant { Name = EntryName.Text, FirstName = EntryFirstName.Text };
                await _connection.InsertAsync(merchant);
            }
            
            //_Merchants.Add(merchant);
            await Navigation.PopAsync();
        }
        private async void OnCancel(object sender, EventArgs e)
        {

        }



    }
}