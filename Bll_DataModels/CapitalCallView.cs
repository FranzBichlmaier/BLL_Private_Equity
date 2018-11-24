using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_DataModels
{
    public class CapitalCallView: INotifyPropertyChanged
    {
        private InvestorCommitment investorCommitment;
        private double callAmount;
        private double callPercent;
        private double openCommitment;
        private string description;
        private int cashFlowNumber;

        public double CallPercent
        {
            get
            {
                return this.callPercent;
            }
            set
            {
                this.callPercent = value;
                OnPropertyChanged("CallPercent");
            }
        }

        public int CashFlowNumber
        {
            get
            {
                return this.cashFlowNumber;
            }
            set
            {
                this.cashFlowNumber = value;
                OnPropertyChanged("CashFlowNumber");
            }
        }

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

        public double OpenCommitment
        {
            get
            {
                return this.openCommitment;
            }
            set
            {
                this.openCommitment = value;
                OnPropertyChanged("OpenCommitment");
            }
        }

        public InvestorCommitment InvestorCommitment
        {
            get
            {
                return this.investorCommitment;
            }
            set
            {
                this.investorCommitment = value;
                OnPropertyChanged("InvestorCommitment");
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
                OnPropertyChanged("CallAmount");
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
