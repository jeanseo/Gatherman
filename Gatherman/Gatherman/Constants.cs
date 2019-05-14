﻿using Xamarin.Forms;

namespace Gatherman
{
    public static class Constants
    {
        private const string BaseAddress = "http://jean-surface:3000/api/";
        public const string PostMerchantURL = BaseAddress + "Merchants";
        public const string PostPictureURL = BaseAddress + "containers/photos/upload";
        public const string KEY_LASTSYNC = "lastSync";

    }
}