using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BLL_DataModels
{
    public class PeActionCapitalCall: INotifyPropertyChanged
    {
        private int peFundId;
        private int callNumber;
        private double callPercentage;
        private double callAmount;
        private double fundVolume;
        private bool allowPreviousDate = false;
        private DateTime dueDate;
        private string description;
        
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
                OnPropertyChanged("Description");
            }
        }

        public double FundVolume
        {
            get
            {
                return this.fundVolume;
            }
            set
            {
                this.fundVolume = value;
                OnPropertyChanged("FundVolume");
            }
        }

        public bool AllowPreviousDate
        {
            get
            {
                return this.allowPreviousDate;
            }
            set
            {
                this.allowPreviousDate = value;
                OnPropertyChanged("AllowPreviousDate");
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

        public int CallNumber
        {
            get
            {
                return this.callNumber;
            }
            set
            {
                this.callNumber = value;
                OnPropertyChanged ( "CallNumber");
            }
        }

        public double CallPercentage
        {
            get
            {
                return this.callPercentage;
            }
            set
            {
                this.callPercentage = value;
                OnPropertyChanged("CallPercentage");

                if (this.CallAmount ==0)
                {
                    this.CallAmount = Math.Round(this.FundVolume * this.CallPercentage/100, 2);
                }              
            }
        }

        public double CallAmount
        {
            get
            {
                return this.callAmount;
            }
            set
            {
                this.callAmount = value;                

                if (this.callPercentage ==0)
                {
                    this.CallPercentage = Math.Round(this.CallAmount / this.FundVolume*100, 4);
                }
                else
                {
                    double deviation = this.FundVolume * 0.01 / 100;      // possible deviation is below 0,01%
                    double theoreticalCallAmount = Math.Round(this.FundVolume*this.callPercentage/100,2); 
                    if ( this.CallAmount > theoreticalCallAmount+deviation || this.CallAmount < theoreticalCallAmount-deviation)
                    {
                        throw new ValidationException("Betrag passt nicht zu Prozentsatz");
                    }
                }
                OnPropertyChanged("CallAmount");
            }
        }

        public DateTime DueDate
        {
            get
            {
                return this.dueDate;
            }
            set
            {
                this.dueDate = value;
                if (this.AllowPreviousDate == false)
                {
                    if (this.dueDate <= DateTime.Now)
                    {
                        throw new ValidationException("Der Fälligkeitstag von Capital Calls kann nicht in der Vergangenheit liegen.");
                    }
                }
                OnPropertyChanged("DueDate");
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
