using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Gatherman.Data
{
    public interface ISQLiteDB
    {
        SQLiteAsyncConnection GetConnection();
    }
}
