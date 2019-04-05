using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System.IO;
using Gatherman.Data;
using Xamarin.Forms;
using Gatherman.Droid.Data;


[assembly : Dependency (typeof(SQLiteDB))]
namespace Gatherman.Droid.Data
{
    public class SQLiteDB : ISQLiteDB
    {
        SQLiteAsyncConnection ISQLiteDB.GetConnection()
        {
            var documentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentPath, "STLayout.db3");
            return new SQLiteAsyncConnection(path);
        }
    }
}