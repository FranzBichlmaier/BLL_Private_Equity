using System;
using System.ComponentModel;

namespace BLL_DataModels
{
    public class PeFundPcap: INotifyPropertyChanged
    {
        private int id;       
        private int peFundId;
        private DateTime asOfDate;
        private double estimatedPcapAmount;
        private DateTime dateOfFinalPcap;
        private double finalPcapAmount;

        public virtual PeFund PeFund { get; set; }

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

        public int PeFundId
        {
            get
            {
                return this.peFundId;
            }
            set
            {
                this.peFundId = value;
                OnPropertyChanged("PeFundId");

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
                this.asOfDate = value;
                OnPropertyChanged("AsOfDate");

            }
        }

        public double EstimatedPcapAmount
        {
            get
            {
                return this.estimatedPcapAmount;
            }
            set
            {
                this.estimatedPcapAmount = value;
                OnPropertyChanged("EstimatedPcapAmount");

            }
        }

        public DateTime DateOfFinalPcap
        {
            get
            {
                return this.dateOfFinalPcap;
            }
            set
            {
                this.dateOfFinalPcap = value;
                OnPropertyChanged("DateOfFinalPcap");

            }
        }

        public double FinalPcapAmount
        {
            get
            {
                return this.finalPcapAmount;
            }
            set
            {
                this.finalPcapAmount = value;
                OnPropertyChanged("FinalPcapAmount");

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
