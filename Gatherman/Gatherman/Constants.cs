using Xamarin.Forms;

namespace Gatherman
{
    public static class Constants
    {
        private const string BaseAddress = "http://jean-surface:3000/api/";
        public const string AccessToken = "?access_token=";
        public const string PostMerchantURL = BaseAddress + "Merchants" + AccessToken;
        public const string PostPushURL = BaseAddress + "Merchants/push" + AccessToken;
        public const string PostPictureURL = BaseAddress + "containers/photos/upload" + AccessToken;
        public const string GetPictureURL = BaseAddress + "containers/photos/download/";
        public const string PostLoginURL = BaseAddress + "agents/login";
        public const string PostLogoutURL = BaseAddress + "agents/logout";
        public const string GetUserURL = BaseAddress + "agents/";
        public const string GetMarketURL = BaseAddress + "markets/"+AccessToken;
        public const string KEY_LASTSYNC = "lastSync";
        public const string KEY_CONNECTED = "connected";
        public const string KEY_CREDENTIALS = "credentials";
        public const string KEY_ISOFFLINE = "isOffline";
        public const bool isOffline =true;
        //ICONES
        public const string PersonAdd = "\ue7fe";
    }
}
