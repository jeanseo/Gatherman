using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Xamarin.Forms;
using Newtonsoft.Json;

namespace Gatherman.DataAccess.Model
{
    public class Merchant : INotifyPropertyChanged
    {
        [PrimaryKey]
        public Guid id { get; set;}

        private string _lastName;

        [MaxLength(255)]
        public string lastName {
            get { return _lastName; }
            set
            {
                if (_lastName == value)
                {
                    //aucune modification
                    return;
                }
                //Il y a eu modification
                _lastName = value;
                
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

        [JsonIgnore]
        public string FullName
        {
            get { return string.Format("{0} {1}", firstName, lastName); }
        }
        private string _pictureFileName;

        public string pictureFileName
        {
            get { return _pictureFileName; }

            set
            {
                if (_pictureFileName == value)
                {
                    //aucune modification
                    return;
                }
                //Il y a eu modification
                _pictureFileName = value;

                OnPropertyChanged();
            }
        }

        private string _pictureLocalPath;
        [MaxLength(255)][JsonIgnore]
        public string pictureLocalPath
        {
            get { return _pictureLocalPath; }

            set
            {
                if (_pictureLocalPath == value)
                {
                    //aucune modification
                    return;
                }
                //Il y a eu modification
                _pictureLocalPath = value;

                OnPropertyChanged();
            }
        }
        [JsonIgnore]
        public ImageSource pictureFullPath
        {
            get
            {
                if (this.pictureFileName == null)
                {
                    return ImageSource.FromResource("Gatherman.images.default_portrait.png");
                }
                    
                else
                    return ImageSource.FromFile(this.pictureLocalPath + "/" + this.pictureFileName);
            }
        }

        public DateTime lastUpdated { get; set; }
        public DateTime creationDate { get; set; }
        [MaxLength(255)]
        public string email { get; set;}
        [MaxLength(255)]
        public string phone { get; set; }

        public int? marketId { get; set; }
        public float incoming { get; set; }
        public float holidays { get; set; }



        private Boolean _deleted;
        public Boolean deleted {
            get { return _deleted; }
            set
            {
                if (_deleted == value) return;
                _deleted = value;
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