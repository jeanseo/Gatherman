using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Gatherman
{
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            Models.User user = new Models.User();
            InitializeComponent();
            Detail = new NavigationPage(new Gatherman.DataAccess.DBAccess(user));
            IsPresented = false;
        }
    }
}
