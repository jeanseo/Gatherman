using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using SQLite;
using System.IO;
using Gatherman.Data;
using Gatherman.iOS.Data;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLiteDB))]
namespace Gatherman.iOS.Data
{
    class SQLiteDB : ISQLiteDB
    {
        SQLiteAsyncConnection ISQLiteDB.GetConnection()
        {
            var documentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentPath, "STLayout.db3");
            return new SQLiteAsyncConnection(path);
        }
    }
}