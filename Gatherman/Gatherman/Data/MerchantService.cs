using Gatherman.DataAccess.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Gatherman.Data
{
    public class MerchantService
    {
        public async Task syncMerchant(Merchant merchant)
        {
            //Ouverture de la connection
            using (var client = new HttpClient())
            {
                //Ajout du Merchant
                string content = JsonConvert.SerializeObject(merchant);
                var response = await client.PostAsync(Constants.PostMerchantURL, new StringContent(content, Encoding.UTF8, "application/json"));
                Debug.Write(response);
            }
            
            //Upload de l'image
            if (merchant.picturePath != null)
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
        }

    }
}
