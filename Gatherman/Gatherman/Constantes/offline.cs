using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Gatherman.Constantes
{
    class offline
    {
        public static bool isOffline
        {
            get
            {
                if (!Application.Current.Properties.ContainsKey(Constants.KEY_ISOFFLINE))
                {
                    return false;
                }
                else
                    return (bool)Application.Current.Properties[Constants.KEY_ISOFFLINE];
            }
            set
            {
                Application.Current.Properties[Constants.KEY_ISOFFLINE] = (bool)value;
                Application.Current.SavePropertiesAsync();
            }
        }
    }
}
