using Xamarin.Forms;

namespace Gatherman
{
    public static class Constants
    {
        private static string BaseAddress = "http://jean-surface:3000/api/";
        public static string PostMerchantURL = BaseAddress + "Merchants";
        public static string PostPictureURL = BaseAddress + "containers/photos/upload";
    }
}
