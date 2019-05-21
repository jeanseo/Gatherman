using System;
using System.Collections.Generic;
using System.Text;
using Gatherman.ThemeResources;
using Xamarin.Forms;

namespace Gatherman
{
    class ThemeManager
    {
        //Définition des différents thèmes
        public enum Themes
        {
            Red
        }



        public static void ChangeTheme(Themes theme)
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();

                //TODO Enregistrer le thème choisi
                

                switch (theme)
                {
                    case Themes.Red:
                        {
                            mergedDictionaries.Add(new RedTheme());
                            break;
                        }
                    default:
                        mergedDictionaries.Add(new RedTheme());
                        break;
                }
            }
        }

        /// Reads current theme id from the local storage and loads it.
        /// </summary>
        public static void LoadTheme()
        {
            Themes currentTheme = CurrentTheme();
            ChangeTheme(currentTheme);
        }

        public static Themes CurrentTheme()
        {
            //TODO Retourner le theme enregistré dans les parametres
            return (Themes)Themes.Red;
        }
    }
}
