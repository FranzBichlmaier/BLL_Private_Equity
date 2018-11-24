using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL_DataModels
{
    public class InvestorCommitment: INotifyPropertyChanged
    {
        private int id;
        private int? investorId;
        private int? bankAccountId;
        private int peFundId;
        private double commitmentAmount;
        private DateTime? dateCommitmentAccepted;
        private double commitmentPlannedAmount;
        private double openCommitment;
        private string peFundReference;
        private double totalCapitalCalls;
        private double totalDistributions;
        private double lastNav;
        private DateTime lastNavDate;
        private double irr;
        private double tvpi;
        private double dpi;

        [JsonIgnore]
        public virtual Investor Investor { get; set; }
        [JsonIgnore]
        public virtual BankAccount BankAccount { get; set; }
        [JsonIgnore]
        public virtual PeFund PeFund { get; set; }
        [JsonIgnore]
        public virtual List<InvestorCommitmentDetail> InvestorCommitmentDetails { get; set; }
        [JsonIgnore]
        public virtual List<InvestorCashFlow> InvestorCashFlows { get; set; }
        [JsonIgnore]
        public virtual List<InvestorPcap> InvestorPcaps { get; set; }


        public InvestorCommitment()
        {
            InvestorCommitmentDetails = new List<InvestorCommitmentDetail>();
            InvestorCashFlows = new List<InvestorCashFlow>();
            InvestorPcaps = new List<InvestorPcap>();
        }
        /// <summary>
        /// creates a copy of InvestorCommitment
        /// </summary>
        /// <param name="copy"></param>
        public InvestorCommitment(InvestorCommitment copy)
        {
            Id = copy.Id;
            InvestorId = copy.InvestorId;
            BankAccountId = copy.BankAccountId;
            PeFundId = copy.PeFundId;
            CommitmentAmount = copy.CommitmentAmount;
            DateCommitmentAccepted = copy.DateCommitmentAccepted;
            CommitmentPlannedAmount = copy.CommitmentPlannedAmount;
            OpenCommitment = copy.OpenCommitment;
            PeFundReference = copy.PeFundReference;
            TotalCapitalCalls = copy.TotalCapitalCalls;
            TotalDistributions = copy.TotalDistributions;
            LastNav = copy.LastNav;
            lastNavDate = copy.LastNavDate;
            Irr = copy.Irr;
            Tvpi = copy.Tvpi;
            Dpi = copy.Dpi;
            InvestorCommitmentDetails = copy.InvestorCommitmentDetails;
            InvestorCashFlows = copy.InvestorCashFlows;
            InvestorPcaps = copy.InvestorPcaps;
        }


        [NotMapped]
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

        public DateTime? DateCommitmentAccepted
        {
            get
            {
                return this.dateCommitmentAccepted;
            }
            set
            {
                this.dateCommitmentAccepted = value;
                OnPropertyChanged("DateCommitmentAccepted");
            }
        }

        public double CommitmentPlannedAmount
        {
            get
            {
                return this.commitmentPlannedAmount;
            }
            set
            {
                this.commitmentPlannedAmount = value;
                OnPropertyChanged("CommitmentPlannedAmount");
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

        public int? InvestorId
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
        [MaxLength (100)]
        public string PeFundReference
        {
            get
            {
                return this.peFundReference;
            }
            set
            {
                peFundReference = value;
                OnPropertyChanged("PeFundReference");
            }
        }
        [NotMapped]
        public double TotalCapitalCalls
        {
            get
            {
                return this.totalCapitalCalls;
            }
            set
            {
                this.totalCapitalCalls = value;
                OnPropertyChanged("TotalCapitalCalls");
            }
        }
        [NotMapped]
        public double TotalDistributions
        {
            get
            {
                return this.totalDistributions;
            }
            set
            {
                this.totalDistributions = value;
                OnPropertyChanged("TotalDistributions");
            }
        }
        [NotMapped]
        public double LastNav { get { return this.lastNav; } set { this.lastNav = value; OnPropertyChanged("LastNav"); } }
        [NotMapped]
        public DateTime LastNavDate { get { return this.lastNavDate; } set { this.lastNavDate = value; OnPropertyChanged("LastNavDate"); } }
        [NotMapped]
        public double Irr { get { return this.irr; } set { this.irr = value; OnPropertyChanged("Irr"); } }
        [NotMapped]
        public double Tvpi { get { return this.tvpi; } set { this.tvpi = value; OnPropertyChanged("Tvpi"); } }
        [NotMapped]
        public double Dpi { get { return this.dpi; } set { this.dpi = value; OnPropertyChanged("Dpi"); } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null) return;
            PropertyChangedEventHandler newHandler = PropertyChanged;
            newHandler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
