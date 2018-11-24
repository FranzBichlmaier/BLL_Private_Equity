using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_DataModels
{
   public class Currency: INotifyPropertyChanged
    {
       private int id;
       private string currencyShortName;
       private string currencyName;

        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
                OnPropertyChanged("Id");
            }
        }

        public string CurrencyShortName
        {
            get
            {
                return this.currencyShortName;
            }
            set
            {
                this.currencyShortName = value;
                OnPropertyChanged("CurrencyShortName");
            }
        }

        public string CurrencyName
        {
            get
            {
                return this.currencyName;
            }
            set
            {
                this.currencyName = value;
                OnPropertyChanged("CurrencyName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

       private void OnPropertyChanged(string propertyName)
       {
           if (PropertyChanged == null) return;
           PropertyChangedEventHandler newHandler = PropertyChanged;
           newHandler(this, new PropertyChangedEventArgs(propertyName));
       }
    }
}
