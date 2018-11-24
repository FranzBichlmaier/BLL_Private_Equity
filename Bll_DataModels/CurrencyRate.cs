using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL_DataModels
{
    public class CurrencyRate: INotifyPropertyChanged
    {
        private int id;
        private DateTime asOfDate;
        private int peFundCurrencyId;
        private int investorCurrencyId;
        private double rate;

        [ForeignKey ("PeFundCurrencyId")]
        public Currency PeFundCurrency { get; set; }
        [ForeignKey ("InvestorCurrencyId")]
        public Currency InvestorCurrency { get; set; }

        public CurrencyRate()
        {
            rate = 1;
        }

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
        public DateTime AsOfDate
        {
            get
            {
                return this.asOfDate;
            }
            set
            {
                this.asOfDate= value;
                OnPropertyChanged("AsOfDate");
            }
        }
        public int PeFundCurrencyId
        {
            get
            {
                return this.peFundCurrencyId;
            }
            set
            {
                this.peFundCurrencyId = value;
                OnPropertyChanged("PeFundCurrencyId");
            }
        }

        public int InvestorCurrencyId
        {
            get
            {
                return this.investorCurrencyId;
            }
            set
            {
                this.investorCurrencyId = value;
                OnPropertyChanged("InvestorCurrencyId");
            }
        }

        public double Rate
        {
            get
            {
                return this.rate;
            }
            set
            {
                this.rate = value;
                OnPropertyChanged("Rate");
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
