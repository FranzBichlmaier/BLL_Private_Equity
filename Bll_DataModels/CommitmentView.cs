using System;
using System.ComponentModel;

namespace BLL_DataModels
{
    public class CommitmentView:INotifyPropertyChanged
    {
        private Investor investor = new Investor();
        private int investorId;
        private int peFundId;
        private double commitmentAmount = 0;
        private DateTime commitmentDate;
        private int? bankAccountId;
        private string activity = string.Empty;
        private double secondaryPurchasePrice = 0;
        private double secondaryOpenCommitment = 0;
        private DateTime secondaryCutOffDate;
        private double secondaryCallsAfterCutOff;
        private double secondaryDistributionsAfterCutOff;

        private InvestorCommitment investorCommitment = new InvestorCommitment();

        public DateTime SecondaryCutOffDate
        {
            get
            {
                return this.secondaryCutOffDate;
            }
            set
            {
                this.secondaryCutOffDate = value;
                OnPropertyChanged("SecondaryCutOffDate");
            }
        }

        public double SecondaryCallsAfterCutOff
        {
            get
            {
                return this.secondaryCallsAfterCutOff;
            }
            set
            {
                this.secondaryCallsAfterCutOff = value;
                OnPropertyChanged("SecondaryCallsAfterCutOff");
            }
        }

        public double SecondaryDistributionsAfterCutOff
        {
            get
            {
                return this.secondaryDistributionsAfterCutOff;
            }
            set
            {
                this.secondaryDistributionsAfterCutOff = value;
                OnPropertyChanged("SecondaryDistributionsAfterCutOff");
            }
        }

        public int? BankAccountId
        {
            get
            {
                return this.bankAccountId;
            }
            set
            {
                this.bankAccountId = value;
                OnPropertyChanged("BankAccountId");
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

        public double SecondaryPurchasePrice
        {
            get
            {
                return this.secondaryPurchasePrice;
            }
            set
            {
                this.secondaryPurchasePrice = value;
                OnPropertyChanged("SecondaryPurchasePrice");
            }
        }

        public double SecondaryOpenCommitment
        {
            get
            {
                return this.secondaryOpenCommitment;
            }
            set
            {
                this.secondaryOpenCommitment = value;
                OnPropertyChanged("SecondaryOpenCommitment");
            }
        }

        public string Activity
        {
            get
            {
                return this.activity;
            }
            set
            {
                this.activity = value;
                OnPropertyChanged("Activity");
            }
        }

        public Investor Investor
        {
            get
            {
                return this.investor;
            }
            set
            {
                this.investor = value;
                OnPropertyChanged("Investor");
            }
        }

        public int InvestorId
        {
            get
            {
                return this.investorId;
            }
            set
            {
                this.investorId = value;
                OnPropertyChanged("InvestorId");
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

        public DateTime CommitmentDate
        {
            get
            {
                return this.commitmentDate;
            }
            set
            {
                this.commitmentDate = value;
                OnPropertyChanged("CommitmentDate");
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null) return;
            PropertyChangedEventHandler newHandler = PropertyChanged;
            newHandler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
