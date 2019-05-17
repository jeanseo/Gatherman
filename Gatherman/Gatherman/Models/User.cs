using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Gatherman.Models
{
    public class User
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }

        public async Task<bool> isAuthenticated()
        {
            Thread.Sleep(5000); // Do work
            return true;
        }

    }

}
