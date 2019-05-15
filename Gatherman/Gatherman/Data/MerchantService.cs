using Gatherman.DataAccess.Model;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Gatherman.Data
{
    public class MerchantService
    {
        //Déclaration de la liaison avec la base de données
        private SQLiteAsyncConnection _connection;
        private DateTime lastSync;

        private struct SyncApiReturnObject
        {
            public List<Merchant> toInsert { get; set; }
            public List<Merchant> toUpdate { get; set; }
        }

        private struct SyncApiPostObject
        {
            public DateTime lastSync { get; set; }
            public List<Merchant> localChanges { get; set; }
        }

        public async Task initializeMerchantList(Action<List<Merchant>> action)
        {

            
            //On récupère les données depuis l'API
            //Ouverture de la connection et requête
            HttpResponseMessage response = null;
            using (var client = new HttpClient())
            {
                response = await client.GetAsync("http://jean-surface:3000/api/Merchants?filter=%7B%22where%22%3A%7B%22deleted%22%3A%22false%22%7D%7D");
                response.EnsureSuccessStatusCode();
            }
            string responseBody = await response.Content.ReadAsStringAsync();
            Debug.Write(responseBody);
            var merchantList = new List<Merchant>();
            merchantList = JsonConvert.DeserializeObject<List<Merchant>>(responseBody);
            //Connection et ajout à la BDD locale
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            await _connection.CreateTableAsync<Merchant>();
            await _connection.InsertAllAsync(merchantList);
            //On vérifie que la donnée lastSync existe, dans le cas contraire on la crée
            Application.Current.Properties[Constants.KEY_LASTSYNC] = DateTime.UtcNow;
            await Application.Current.SavePropertiesAsync();
            Debug.Write("fin de l'initialisation");

            Device.BeginInvokeOnMainThread(() => {
                    action.Invoke(merchantList);
                });

        }

        public async Task syncMerchant(Action<List<Merchant>> action)
        {
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            await _connection.CreateTableAsync<Merchant>();

            //On récupère la date de la dernière synchronisation
            if (Application.Current.Properties.ContainsKey(Constants.KEY_LASTSYNC))
            {
                lastSync = (DateTime)Application.Current.Properties[Constants.KEY_LASTSYNC];
            }
            //Déclaration d'une liste local change
            var postChanges = new SyncApiPostObject();
            postChanges.lastSync = lastSync;

            var localChanges = new List<Merchant>();
            //Connection à la BDD locale
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            localChanges.AddRange(await _connection.QueryAsync<Merchant>("SELECT * FROM Merchant WHERE lastUpdated > ?", lastSync));
            Debug.Write("\n"+JsonConvert.SerializeObject(localChanges));
            postChanges.localChanges = localChanges;

            //Ouverture de la connection et requête
            HttpResponseMessage response = null;
            using (var client = new HttpClient())
            {

                //Push vers l'API
                string content = JsonConvert.SerializeObject(postChanges);
                Debug.Write(content);

                response = await client.PostAsync("http://jean-surface:3000/api/Merchants/push", new StringContent(content, Encoding.UTF8, "application/json"));
            }
                    // Handle success
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<SyncApiReturnObject>(responseBody);
                        
            //Faire les requêtes insert et update en local
            if (responseObject.toUpdate!=null)
                await _connection.UpdateAllAsync(responseObject.toUpdate);
            if (responseObject.toInsert != null)
                await _connection.InsertAllAsync(responseObject.toInsert);



            // Requête delete, on récupère dans la liste insert ceux qui sont deleted, et on les delete de la base locale
            //on recherche les objets à supprimer

            await _connection.Table<Merchant>().Where(x => x.deleted == true).DeleteAsync();

            //TODO Gerer les envois d'images
            //Upload de l'image
            /*if (merchant.picturePath != null)
            {
                Uri uri = new Uri(Constants.PostPictureURL);
                using (var webclient = new WebClient())
                {
                    webclient.UploadFileCompleted += new UploadFileCompletedEventHandler((object sender2, UploadFileCompletedEventArgs e2) =>{
                        Debug.Write(e2);
                    });

                    try
                    {
                        webclient.UploadFileAsync(uri, merchant.picturePath);

                    }
                    catch (Exception ex)
                    {
                        //await DisplayAlert("Erreur", "Une erreur réseau s'est produite: " + ex.Message, "OK");
                    }
                }

            }
            */
            //On met à jour la date de lastSync
            Application.Current.Properties[Constants.KEY_LASTSYNC] = DateTime.UtcNow;
            await Application.Current.SavePropertiesAsync();
            var MerchantList = new List<Merchant>();
            MerchantList.AddRange(await _connection.QueryAsync<Merchant>("SELECT * FROM Merchant WHERE deleted=?", false));
            Debug.Write("\n" + JsonConvert.SerializeObject(MerchantList));

            Device.BeginInvokeOnMainThread(() => {
                action.Invoke(MerchantList);
            });

        }

    }
}
