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
        private struct LoginAPIPostObject
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        private struct LoginAPIResponseObject
        {
            public string id { get; set; }
            public int ttl { get; set; }
            public DateTime created { get; set; }
            public int userId { get; set; }
        }

        private struct _user
        {
            public string username { get; set; }
            public string email { get; set; }
            public string password { get; set; }
            public string id { get; set; }
            public int ttl { get; set; }
            public DateTime created { get; set; }
            public int userId { get; set; }
        }


        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string id { get; set; }
        public int ttl { get; set; }
        public DateTime created { get; set; }
        public int userId { get; set; }

        public User()
        {
            //TODO Récupérer les identifiants stockés
            if (Application.Current.Properties.ContainsKey(Constants.KEY_LASTSYNC))
            {
                string jsonUser = (string)Application.Current.Properties[Constants.KEY_CREDENTIALS];
                Debug.Write("-----DONNEES STOCKEES------\n" + jsonUser);
                _user existingUser = JsonConvert.DeserializeObject<_user>(jsonUser);
                this.username = existingUser.username;
                this.email = existingUser.email;
                this.password = existingUser.password;
                this.id = existingUser.id;
                this.ttl = existingUser.ttl;
                this.created = existingUser.created;
                this.userId = existingUser.userId;
            }

        }

        public async Task<int> isAuthenticated()
        {
            return 200;
            HttpResponseMessage response = null;
            LoginAPIPostObject body = new LoginAPIPostObject { username = this.username, password = this.password };
            try
            {


                using (var client = new HttpClient())
                {
                    //Push vers l'API
                    Debug.Write(this.username);
                    string content = JsonConvert.SerializeObject(this);
                    Debug.Write("--------Requête JSON-------" + content);

                    response = await client.PostAsync(Constants.PostLoginURL, new StringContent(content, Encoding.UTF8, "application/json"));
                }
                if (response.IsSuccessStatusCode)
                {
                    // Handle success
                    string responseBody = await response.Content.ReadAsStringAsync();
                    LoginAPIResponseObject loggedUser = JsonConvert.DeserializeObject<LoginAPIResponseObject>(responseBody);
                    //On réécrie les username et password qui ont été effacés par le retour de la requête
                    id = loggedUser.id;
                    ttl = loggedUser.ttl;
                    created = loggedUser.created;
                    created = loggedUser.created;
                    return 200;
                }
                else
                {
                    return (int)response.StatusCode;
                }
                

            }
            catch (Exception ex)
            {
                Debug.Write("--------ERREUR---------\n" + ex);

            }
            return 0;

        }

    }

}
