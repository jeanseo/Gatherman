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

        public async Task initializeMerchantList()
        {

            if (!Application.Current.Properties.ContainsKey(Constants.KEY_LASTSYNC) || Application.Current.Properties[Constants.KEY_LASTSYNC] == null)
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
                var merchantList = JsonConvert.DeserializeObject<List<Merchant>>(responseBody);
                //Connection et ajout à la BDD locale
                _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
                await _connection.CreateTableAsync<Merchant>();
                await _connection.InsertAllAsync(merchantList);
                //On vérifie que la donnée lastSync existe, dans le cas contraire on la crée
                Application.Current.Properties[Constants.KEY_LASTSYNC] = DateTime.UtcNow;
                await Application.Current.SavePropertiesAsync();
                Debug.Write("fin de l'initialisation");
                }
         }

        public async Task syncMerchant()
        {
            //On récupère la date de la dernière synchronisation
            if (Application.Current.Properties.ContainsKey(Constants.KEY_LASTSYNC))
            {
                lastSync = (DateTime)Application.Current.Properties[Constants.KEY_LASTSYNC];
            }
            //Déclaration d'une liste local change
            var localChanges = new List<Merchant>();

            localChanges.Add(new Merchant{ lastUpdated = lastSync});

            //Connection à la BDD locale
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            localChanges.AddRange(await _connection.QueryAsync<Merchant>("SELECT * FROM Merchant WHERE lastUpdated > ?", lastSync));
            //Debug.Write("\n"+JsonConvert.SerializeObject(localChanges)+"\n"+lastSync+"\n");

            //Ouverture de la connection et requête
            HttpResponseMessage response = null;
            using (var client = new HttpClient())
            {

                //Push vers l'API
                string content = JsonConvert.SerializeObject(localChanges);

                response = await client.PostAsync("http://jean-surface:3000/api/Merchants/push", new StringContent(content, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
            }   
            string responseBody = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<SyncApiReturnObject> (responseBody);            
            //Faire les requêtes insert et update en local

            await _connection.UpdateAllAsync(responseObject.toUpdate);
            await _connection.InsertAllAsync(responseObject.toInsert);

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

        }

    }
}
