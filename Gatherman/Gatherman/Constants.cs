using Xamarin.Forms;

namespace Gatherman
{
    public static class Constants
    {
        private const string BaseAddress = "http://jean-surface:3000/api/";
        private const string AccessToken = "?access_token=";
        public const string PostMerchantURL = BaseAddress + "Merchants" + AccessToken;
        public const string PostPushURL = BaseAddress + "Merchants/push" + AccessToken;
        public const string PostPictureURL = BaseAddress + "containers/photos/upload" + AccessToken;
        public const string PostLoginURL = BaseAddress + "Users/login";
        public const string KEY_LASTSYNC = "lastSync";
        public const string KEY_CONNECTED = "connected";
        public const string KEY_CREDENTIALS = "credentials";
        public const bool isOffline = true;

    }
}
