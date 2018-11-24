using System;
using System.ComponentModel;

namespace BLL_DataModels
{
    public class PcapView: INotifyPropertyChanged
    {
        private InvestorCommitment investorCommitment;
        private DateTime lastQuarterEnd;
        private double lastQuarterNav;
        private DateTime navQuarterEnd;
        private double estimatedNav;
        private double quarterDistributions;
        private double quarterContributions;
        private DateTime finalNavDate;
        private double finalNav;
        private double finalNavInInvestorCurrency;
        private double commitmentAmount;
        private double  currentEstimatedNav;

        public double  CurrentEstimatedNav
        {
            get { return currentEstimatedNav; }
            set { currentEstimatedNav = value; OnPropertyChanged("CurrentEstimatedNav"); }
        }

        private double currentEstimatedNavInvestorCurrency;

        public double CurrentEstimatedNavInvestorCurrency
        {
            get { return currentEstimatedNavInvestorCurrency; }
            set { currentEstimatedNavInvestorCurrency = value; OnPropertyChanged("CurrentEstimatedNavInvestorCurrency"); }
        }


        public double FinalNavInInvestorCurrency
        {
            get
            {
                return this.finalNavInInvestorCurrency;
            }
            set
            {
                this.finalNavInInvestorCurrency = value;
                OnPropertyChanged("FinalNavInInvestorCurrency");
            }
        }

        public double CommitmentAmount
        {
            get
            {
                return this.commitmentAmount;
            }
            set
            {
                this.commitmentAmount = value;
                OnPropertyChanged("CommitmentAmount");
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

        public DateTime LastQuarterEnd
        {
            get
            {
                return this.lastQuarterEnd;
            }
            set
            {
                this.lastQuarterEnd = value;
                OnPropertyChanged("LastQuarterEnd");
            }
        }

        public double LastQuarterNav
        {
            get
            {
                return this.lastQuarterNav;
            }
            set
            {
                this.lastQuarterNav = value;
                OnPropertyChanged("LastQuarterNav");
            }
        }

        public DateTime NavQuarterEnd
        {
            get
            {
                return this.navQuarterEnd;
            }
            set
            {
                this.navQuarterEnd = value;
                OnPropertyChanged("NavQuarterEnd");
            }
        }

        public double EstimatedNav
        {
            get
            {
                return this.estimatedNav;
            }
            set
            {
                this.estimatedNav = value;
                OnPropertyChanged("EstimatedNav");
            }
        }

        public double QuarterDistributions
        {
            get
            {
                return this.quarterDistributions;
            }
            set
            {
                this.quarterDistributions = value;
                OnPropertyChanged("QuarterDistributions");
            }
        }

        public double QuarterContributions
        {
            get
            {
                return this.quarterContributions;
            }
            set
            {
                this.quarterContributions = value;
                OnPropertyChanged("QuarterContributions");
            }
        }

        public DateTime FinalNavDate
        {
            get
            {
                return this.finalNavDate;
            }
            set
            {
                this.finalNavDate = value;
                OnPropertyChanged("FinalNavDate");
            }
        }

        public double FinalNav
        {
            get
            {
                return this.finalNav;
            }
            set
            {
                this.finalNav = value;
                OnPropertyChanged("FinalNav");
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
