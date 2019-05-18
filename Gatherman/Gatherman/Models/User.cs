using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
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
        public string id { get; set; }
        public int ttl { get; set; }
        public DateTime created { get; set; }
        public int userId { get; set; }
        

    }

}
