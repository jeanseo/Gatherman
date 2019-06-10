using Gatherman.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Gatherman.ViewModels
{
    class RootModel : INotifyPropertyChanged
    {
        List<Market> marketList;

        public List<Market> MarketList
        {
            get { return marketList; }
            set
            {
                if (marketList != value)
                {
                    marketList = value;
                    OnPropertyChanged();
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
