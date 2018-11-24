using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL_DataModels
{
    public class InvestorPcap: BindableBase
    {
        
        [JsonIgnore]
        public virtual InvestorCommitment InvestorCommitment { get; set; }

        private double finalPcapAmountInInvestorCurrency;
        public double FinalPcapAmountInInvestorCurrency
        {
            get { return this.finalPcapAmountInInvestorCurrency; }
            set { SetProperty(ref finalPcapAmountInInvestorCurrency, value); }
        }

        private int id;
        public int Id
        {
            get { return this.id; }
            set { SetProperty(ref id, value); }
        }

        private int investorCommitmentId;

        public int InvestorCommitmentId
        {
            get { return this.investorCommitmentId; }
            set { SetProperty(ref investorCommitmentId, value); }
        }

        private DateTime asOfDate;

        public DateTime AsOfDate
        {
            get { return this.asOfDate; }
            set { SetProperty(ref asOfDate, value); }
        }

        private double estimatedPcapAmount;

        public double EstimatedPcapAmount
        {
            get { return this.estimatedPcapAmount; }
            set { SetProperty(ref estimatedPcapAmount, value); }
        }

        private DateTime dateOfFinalPcap;

        public DateTime DateOfFinalPcap
        {
            get { return this.dateOfFinalPcap; }
            set { SetProperty(ref dateOfFinalPcap, value); }
        }

        private double finalPcapAmount;

        public double FinalPcapAmount
        {
            get { return this.finalPcapAmount; }
            set { SetProperty(ref finalPcapAmount, value); }
        }

        private string investorReference = string.Empty;
        [NotMapped]
        public string InvestorReference
        {
            get { return this.investorReference; }
            set { SetProperty(ref investorReference, value); }
        }

        private string investorHqTrustAccount = string.Empty;
        [NotMapped]
        public string InvestorHqTrustAccount
        {
            get { return this.investorHqTrustAccount; }
            set { SetProperty(ref investorHqTrustAccount, value); }
        }

        private double  commitmentAmount;
        [NotMapped]
        public double  CommitmentAmount
        {
            get { return this.commitmentAmount; }
            set { SetProperty(ref commitmentAmount, value); }
        }

        private bool isCalculated =false;
        [NotMapped]

        public bool IsCalculated
        {
            get { return this.isCalculated; }
            set { SetProperty(ref isCalculated, value); }
        }
    }
}
