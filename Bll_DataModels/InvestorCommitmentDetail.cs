using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL_DataModels
{
    public class InvestorCommitmentDetail: INotifyPropertyChanged
    {
        private int id;
        private int investorCommitmentId;
        private DateTime commitmentDate;
        private double commitmentAmount;
        private double secondaryPurchaseAmount;
        private double secondaryOpenCommitment;
        private DateTime? secondaryCutOffDate=null;
        private double secondaryCallsAfterCutOff;
        private double secondaryDistributionsAfterCutOff;
        private bool isSecondary;

        [JsonIgnore]
        public virtual InvestorCommitment InvestorCommitment { get; set; }

        public DateTime? SecondaryCutOffDate
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

        public double SecondaryPurchaseAmount
        {
            get
            {
                return this.secondaryPurchaseAmount;
            }
            set
            {
                this.secondaryPurchaseAmount = value;
                OnPropertyChanged("SecondaryPurchaseAmount");
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

        public int InvestorCommitmentId
        {
            get
            {
                return this.investorCommitmentId;
            }
            set
            {
                this.investorCommitmentId = value;
                OnPropertyChanged("InvestorCommitmentId");

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
        [NotMapped]
        public bool IsSecondary
        {
            get
            {
                return isSecondary;
            }
            set
            {
                isSecondary = value;
                OnPropertyChanged("IsSecondary");
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
