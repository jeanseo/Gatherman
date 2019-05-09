using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Gatherman.DataAccess.Model
{
    public class Merchant : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public Guid id { get; set; }

        private string _name;

        [MaxLength(255)]
        public string lastName {
            get { return _name; }
            set
            {
                if (_name == value)
                {
                    //aucune modification
                    return;
                }
                //Il y a eu modification
                _name = value;
                
                OnPropertyChanged();
            }
        }

        private string _firstName;

        [MaxLength(255)]
        public string firstName
        {
            get { return _firstName; }
            set
            {
                if (_firstName == value)
                {
                    //aucune modification
                    return;
                }
                //Il y a eu modification
                _firstName = value;

                OnPropertyChanged();
            }
        }

        public string FullName
        {
            get { return string.Format("{0} {1}", firstName, lastName); }
        }
        private string _portrait;

        public string picturePath
        {
            get { return _portrait; }

            set
            {
                if (_portrait == value)
                {
                    //aucune modification
                    return;
                }
                //Il y a eu modification
                _portrait = value;

                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string PropertyName=null)
        {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}