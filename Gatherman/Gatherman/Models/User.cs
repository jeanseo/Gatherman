using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
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
            public string firstName { get; set; }
            public string lastName { get; set; }
            public DateTime birthDate { get; set; }
            public string birthPlace { get; set; }
            public string phoneNumber { get; set; }
            public string pictureFileName { get; set; }
            public string pictureLocalPath { get; set; }
            public string address { get; set; }
            public string email { get; set; }
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
            public string firstName { get; set; }
            public string lastName { get; set; }
            public DateTime birthDate { get; set; }
            public string birthPlace { get; set; }
            public string phoneNumber { get; set; }
            public string pictureFileName { get; set; }
            public string pictureLocalPath { get; set; }
            public string address { get; set; }
        }


        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        [JsonIgnore]
        public string id { get; set; }
        public int ttl { get; set; }
        public DateTime created { get; set; }
        public int userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateTime birthDate { get; set; }
        public string birthPlace { get; set; }
        public string phoneNumber { get; set; }
        public string pictureFileName { get; set; }
        [JsonIgnore]
        public string pictureLocalPath { get; set; }
        public string address { get; set; }

        [JsonIgnore]
        public string fullName
        {
            get { return string.Format("{0} {1}", firstName, lastName); }
        }

        public ImageSource pictureFullPath
        {
            get
            {
                if (this.pictureFileName == null)
                {
                    return ImageSource.FromResource("Gatherman.images.default_portrait.png");
                }

                else
                    return ImageSource.FromFile(this.pictureLocalPath + "/" + this.pictureFileName);
            }
        }

        private struct lastLoginData
        {
            public Location LastLoginPlace { get; set; }
            public DateTime LastLoginDate
            {
                get
                {
                    return DateTime.UtcNow;
                }
            }
        }


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
                this.firstName = existingUser.firstName;
                this.lastName = existingUser.lastName;
                this.birthDate = existingUser.birthDate;
                this.birthPlace = existingUser.birthPlace;
                this.phoneNumber = existingUser.phoneNumber;
                this.pictureFileName = existingUser.pictureFileName;
                this.pictureLocalPath = existingUser.pictureLocalPath;
                this.address = existingUser.address;
                
    }

        }
        private async Task<Location> GetLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Debug.Write($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                }
                else
                {
                    location = await Geolocation.GetLastKnownLocationAsync();
                }
                return location;
            }
            catch (Exception ex)
            {                
                return null;
            }
        }

        public async Task<int> isAuthenticated()
        {
            HttpResponseMessage response = null;
            LoginAPIPostObject body = new LoginAPIPostObject { username = this.username, password = this.password };
            try
            {
                using (var client = new HttpClient())
                {
                    //Requete de connexion
                    Debug.Write(this.username);
                    string content = JsonConvert.SerializeObject(this);
                    Debug.Write("--------Requête JSON de connexion-------" + content);

                    response = await client.PostAsync(Constants.PostLoginURL, new StringContent(content, Encoding.UTF8, "application/json"));
                    Debug.Write(response);
                }
                if (response.IsSuccessStatusCode)
                {
                    // Handle success
                    string responseBody = await response.Content.ReadAsStringAsync();
                    LoginAPIResponseObject loggedUser = JsonConvert.DeserializeObject<LoginAPIResponseObject>(responseBody);
                    //On récupère les infos renvoyées lors de la connexion
                    id = loggedUser.id;
                    ttl = loggedUser.ttl;
                    userId = loggedUser.userId;
                    created = loggedUser.created;
                    //TODO on lance une requête pour récupérer les informations complètes de l'utilisateur
                    using (var client = new HttpClient())
                    {
                        Debug.Write(Constants.GetUserURL + userId + "?access_token=" + id);
                        response = await client.GetAsync(Constants.GetUserURL + userId + "?access_token=" + id);
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        loggedUser = JsonConvert.DeserializeObject<LoginAPIResponseObject>(responseBody);

                    }
                    
                    firstName = loggedUser.firstName;
                    Debug.Write("firstname"+firstName);
                    lastName = loggedUser.lastName;
                    birthDate = loggedUser.birthDate;
                    birthPlace = loggedUser.birthPlace;
                    phoneNumber = loggedUser.phoneNumber;
                    pictureFileName = loggedUser.pictureFileName;
                    email = loggedUser.email;
                    Debug.Write("email:" + email);
                    address = loggedUser.address;
                    if(pictureFileName!=null || pictureFileName != "")
                    {
                        using (var webclient = new WebClient())
                        {
                            var mainDir = FileSystem.AppDataDirectory;
                            Uri uri = new Uri(Constants.GetPictureURL + pictureFileName + Constants.AccessToken + id);
                            webclient.DownloadFileAsync(uri, mainDir + "/" + pictureFileName);
                            pictureLocalPath = mainDir;
                        }
                    }
                    // Envoi des informations de connexion au serveur
                    updateUser();

                        return 200;
                    //On récupère l'image


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

        private async Task updateUser()
        {
            Location lastLoginPlace  = await GetLocation();
            string content = String.Format("{{\"lastLoginDate\": \"{0:yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'}\",\"lastLoginPlace\": {{\"lat\":{1},\"lng\":{2}}}}}",
                DateTime.UtcNow, JsonConvert.SerializeObject(lastLoginPlace.Latitude), JsonConvert.SerializeObject(lastLoginPlace.Longitude));
            Debug.Write("-----Requete d'update du user-----"+content);
            //Faire une requete PATCH
            string url = Constants.GetUserURL + userId + Constants.AccessToken + id;
            using (var client = new HttpClient())
            {
                var method = new HttpMethod("PATCH");

                var request = new HttpRequestMessage(method, url)
                {
                    Content = new StringContent(
                                    content,
                                    Encoding.UTF8, "application/json")
                };
                var response = await client.SendAsync(request);
                Debug.Write("-----Reponse de la requete PATCH----"+response);
            }
            
            }
        

        public async Task<int> logOff()
        {
            this.username = null;
            this.password = null;
            //on supprime les infos de connection
            Application.Current.Properties[Constants.KEY_CONNECTED] = false;
            Application.Current.Properties[Constants.KEY_CREDENTIALS] = null;
            await Application.Current.SavePropertiesAsync();
            return 204;
            /*
            HttpResponseMessage response = null;
            LoginAPIPostObject body = new LoginAPIPostObject { username = this.username, password = this.password };
            try
            {


                using (var client = new HttpClient())
                {
                    string content = "";
                    response = await client.PostAsync(Constants.PostLogoutURL, new StringContent(content, Encoding.UTF8, "application/json"));
                }
            }
            catch (Exception ex)
            {
                Debug.Write("--------ERREUR---------\n" + ex);

            }
            
            return 0;
            */
                }


            }

}
