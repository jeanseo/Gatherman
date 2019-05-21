using Gatherman.DataAccess.Model;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Gatherman.Data
{
    public class MerchantService
    {
        public Models.User loggedUser;

        //Déclaration de la liaison avec la base de données
        private SQLiteAsyncConnection _connection;
        private DateTime lastSync;

        private struct SyncApiReturnObject
        {
            public List<Merchant> toInsert { get; set; }
            public List<Merchant> toUpdate { get; set; }
            public DateTime lastSync { get; set; }
        }

        private struct SyncApiPostObject
        {
            public DateTime lastSync { get; set; }
            public List<Merchant> localChanges { get; set; }
        }

        public async Task initializeMerchantList(Models.User _user)
        {
            loggedUser = _user;
            //On récupère les données depuis l'API
            //Ouverture de la connection et requête
            HttpResponseMessage response = null;
            using (var client = new HttpClient())
            {
                response = await client.GetAsync("http://jean-surface:3000/api/Merchants?filter=%7B%22where%22%3A%7B%22deleted%22%3A%22false%22%7D%7D&access_token=" + loggedUser.id);
                response.EnsureSuccessStatusCode();
            }
            string responseBody = await response.Content.ReadAsStringAsync();
            var merchantList = new List<Merchant>();
            merchantList = JsonConvert.DeserializeObject<List<Merchant>>(responseBody);
            //Connection et ajout à la BDD locale
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            Debug.Write("------à insérer-------\n" + JsonConvert.SerializeObject(merchantList));
            int rows = await _connection.InsertAllAsync(merchantList);
            Debug.Write("------"+rows+" lignes insérées-------\n" + JsonConvert.SerializeObject(merchantList));
            //On crée la donnée lastSync
            Application.Current.Properties[Constants.KEY_LASTSYNC] = DateTime.UtcNow;
            await Application.Current.SavePropertiesAsync();
            Debug.Write("fin de l'initialisation");
           
        }

        public async Task syncMerchant(Models.User _user)
        {
            loggedUser = _user;
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
            await _connection.CreateTableAsync<Merchant>();

            //On récupère la date de la dernière synchronisation
            if (Application.Current.Properties.ContainsKey(Constants.KEY_LASTSYNC))
            {
                lastSync = (DateTime)Application.Current.Properties[Constants.KEY_LASTSYNC];
            }
            Debug.Write("lastSync: " + lastSync);
            //Déclaration d'une liste local change
            var postChanges = new SyncApiPostObject();

            postChanges.lastSync = lastSync;

            var localChanges = new List<Merchant>();
            //Connection à la BDD locale, on cherche tout ce qui a été modifié en local depuis la dernière synchro
            localChanges.AddRange(await _connection.QueryAsync<Merchant>("SELECT * FROM Merchant WHERE lastUpdated > ?", lastSync));
            Debug.Write("\n"+JsonConvert.SerializeObject(localChanges));
            postChanges.localChanges = localChanges;

            //Ouverture de la connection et requête
            HttpResponseMessage response = null;
            string responseBody = null;
            try
            {
                using (var client = new HttpClient())
                    {

                        //Push vers l'API
                        string content = JsonConvert.SerializeObject(postChanges);
                        Debug.Write("--------Requête JSON-------"+content);
                        Debug.Write("--------Requête JSON-------" + loggedUser.id);

                        response = await client.PostAsync(Constants.PostPushURL + loggedUser.id, new StringContent(content, Encoding.UTF8, "application/json"));
                    }
                            // Handle success
                            responseBody = await response.Content.ReadAsStringAsync();
                Debug.Write("\n------RETOUR JSON DU PUSH------\n" + responseBody);
            }
            catch(Exception ex)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Debug.Write(ex.ToString());
                    Application.Current.MainPage.DisplayAlert("Erreur", "Une erreur s'est produite dans  la récupération de données" + ex.Message, "OK");
                });
            }
            
            //Verification du retour
            //Faire les requêtes insert et update en local

            try
            {
                var responseObject = JsonConvert.DeserializeObject<SyncApiReturnObject>(responseBody);

                Debug.Write("----TO UPDATE------\n" + JsonConvert.SerializeObject(responseObject.toUpdate));
                Debug.Write("----TO INSERT------\n" + JsonConvert.SerializeObject(responseObject.toInsert));
                if (responseObject.toUpdate.Any())
                {
                    Debug.Write("\n-----nombre d'update----" + responseObject.toUpdate.Count());
                    await _connection.UpdateAllAsync(responseObject.toUpdate);
                }
                    
                if (responseObject.toInsert.Any())
                {
                    Debug.Write("\n-----nombre d'insert----" + responseObject.toInsert.Count());
                    await _connection.InsertAllAsync(responseObject.toInsert);
                }
                    
                //On met à jour la date de lastSync
                Debug.Write("\n-------DATE RENVOYEE------\n" + responseObject.lastSync.ToString());
                Application.Current.Properties[Constants.KEY_LASTSYNC] = responseObject.lastSync;
                await Application.Current.SavePropertiesAsync();
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Debug.Write(ex.ToString());
                    Application.Current.MainPage.DisplayAlert("Erreur", "Une erreur s'est produite dans l'écriture de données locales" + ex.Message, "OK");

                });
            }
            
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


        }

    }
}
