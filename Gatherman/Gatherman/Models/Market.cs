using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;


namespace Gatherman.Models 
{
    class Market
    {
        [PrimaryKey]
        public int id { get; set; }
        [MaxLength(255)]
        public string name { get; set; }
        [MaxLength(255)]
        public string city { get; set; }
    }
}
