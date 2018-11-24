using Prism.Mvvm;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL_DataModels
{
    public class ReportInformationCapitalCall: BindableBase
    {
        private int id ;
        public int Id { get => id; set => SetProperty(ref id, value); }
        private int reportId ;
        public int ReportId { get => reportId; set => SetProperty(ref reportId, value); }
        private int peFundId ;
        public int PeFundId { get => peFundId; set => SetProperty(ref peFundId, value); }
        private int investorId ;
        public int InvestorId { get => investorId; set => SetProperty(ref investorId, value); }
        private int investorCommitmentId ;
        public int InvestorCommitmentId { get => investorCommitmentId; set => SetProperty(ref investorCommitmentId, value); }
        private int advisorId ;
        public int AdvisorId { get => advisorId; set => SetProperty(ref advisorId, value); }
        private int bankAccountId ;
        public int BankAccountId { get => bankAccountId; set => SetProperty(ref bankAccountId, value); }
        private DateTime reportDate ;
        public DateTime ReportDate { get => reportDate; set => SetProperty(ref reportDate, value); }
        private DateTime cashFlowDueDate ;
        public DateTime CashFlowDueDate { get => cashFlowDueDate; set => SetProperty(ref cashFlowDueDate, value); }
        private string secondSignature;
        public string SecondSignature { get => secondSignature; set => SetProperty(ref secondSignature, value); }
        private string fundReference;
        public string FundReference { get => fundReference; set => SetProperty(ref fundReference, value); }
        private string cashFlowSubject;
        public string CashFlowSubject { get => cashFlowSubject; set => SetProperty(ref cashFlowSubject, value); }
        private string cashFlowNumberText;
        public string CashFlowNumberText { get => cashFlowNumberText; set => SetProperty(ref cashFlowNumberText, value); }
        private double cashFlowAmount;
        public double CashFlowAmount { get => cashFlowAmount; set => SetProperty(ref cashFlowAmount, value); }
        private double cashFlowAmountInPercent ;
        public double CashFlowAmountInPercent { get => cashFlowAmountInPercent; set => SetProperty(ref cashFlowAmountInPercent, value); }
        private double commitmentAmount ;
        public double CommitmentAmount { get => commitmentAmount; set => SetProperty(ref commitmentAmount, value); }
        private double openCommitment ;
        public double OpenCommitment { get => openCommitment; set => SetProperty(ref openCommitment, value); }
        private string currencyShortName ;
        public string CurrencyShortName { get => currencyShortName; set => SetProperty(ref currencyShortName, value); }
        private string individualHtmlText;
        public string IndividualHtmlText { get => individualHtmlText;set => SetProperty(ref individualHtmlText, value); }
        private string individualHtmlText2;
        public string IndividualHtmlText2 { get => individualHtmlText2; set => SetProperty(ref individualHtmlText2, value); }
        private double cashFlowAmountCall ;
        public double CashFlowAmountCall { get => cashFlowAmountCall; set => SetProperty(ref cashFlowAmountCall, value); }
        private double cashFlowAmountDistribution ;
        public double CashFlowAmountDistribution { get => cashFlowAmountDistribution; set => SetProperty(ref cashFlowAmountDistribution, value); }
        private double returnOfCapital ;
        public double ReturnOfCapital { get => returnOfCapital; set => SetProperty(ref returnOfCapital, value); }
        private double capitalGain ;
        public double CapitalGain { get => capitalGain; set => SetProperty(ref capitalGain, value); }
        private double interests ;
        public double Interests { get => interests; set => SetProperty(ref interests, value); }
        private double dividends ;
        public double Dividends { get => dividends; set => SetProperty(ref dividends, value); }
        private double otherIncome ;
        public double OtherIncome { get => otherIncome; set => SetProperty(ref otherIncome, value); }
        private double recallableAmount ;
        public double RecallableAmount { get => recallableAmount; set => SetProperty(ref recallableAmount, value); }
        private double partnershipExpenses ;
        public double PartnershipExpenses { get => partnershipExpenses; set => SetProperty(ref partnershipExpenses, value); }
        private double withholdingTax ;
        public double WithholdingTax { get => withholdingTax; set => SetProperty(ref withholdingTax, value); }
        private double investorExpenses ;
        public double InvestorExpenses { get => investorExpenses; set => SetProperty(ref investorExpenses, value); }
        private double lookBackInterests ;
        public double LookBackInterests { get => lookBackInterests; set => SetProperty(ref lookBackInterests, value); }
        private int capitalCallNumber ;
        public int CapitalCallNumber { get => capitalCallNumber; set => SetProperty(ref capitalCallNumber, value); }
        private int distributionNumber ;
        public int DistributionNumber { get => distributionNumber; set => SetProperty(ref distributionNumber, value); }
        private double totalDistributionPaidIn ;
        public double TotalDistributionPaidIn { get => totalDistributionPaidIn; set => SetProperty(ref totalDistributionPaidIn, value); }
        private double totalDistributionCommitment ;
        public double TotalDistributionCommitment { get => totalDistributionCommitment; set => SetProperty(ref totalDistributionCommitment, value); }
        private double totalContributionCommitment ;
        public double TotalContributionCommitment { get => totalContributionCommitment; set => SetProperty(ref totalContributionCommitment, value); }
        private double totalDistribution ;
        public double TotalDistribution { get => totalDistribution; set => SetProperty(ref totalDistribution, value); }
        private double totalContribution ;
        public double TotalContribution { get => totalContribution; set => SetProperty(ref totalContribution, value); }
        [NotMapped]
        public Investor Investor { get; set; }
        [NotMapped]
        public InvestorCommitment InvestorCommitment { get; set; }

        public ReportInformationCapitalCall()
        {

        }
        /// <summary>
        /// This constructor creates a new ReportInformationCapitalCall with information of an existing ReportInformationCapitalCall
        /// </summary>
        /// <param name="item">ReportInformationCapitalCall</param>
        public ReportInformationCapitalCall(ReportInformationCapitalCall item)
        {
            id = item.Id;
            fundReference = item.FundReference;
            reportId = item.ReportId;
            peFundId = item.PeFundId;
            investorId = item.InvestorId;
            advisorId = item.AdvisorId;
            bankAccountId = item.BankAccountId;
            reportDate = item.ReportDate;
            cashFlowDueDate = item.CashFlowDueDate;
            capitalCallNumber = item.CapitalCallNumber;
            capitalGain = item.CapitalGain;
            cashFlowAmount = item.cashFlowAmount;
            cashFlowAmountCall = item.cashFlowAmountCall;
            cashFlowAmountDistribution = item.CashFlowAmountDistribution;
            cashFlowAmountInPercent = item.CashFlowAmountInPercent;
            cashFlowNumberText = item.CashFlowNumberText;
            cashFlowSubject = item.CashFlowSubject;
            commitmentAmount = item.commitmentAmount;
            currencyShortName = item.CurrencyShortName;
            distributionNumber = item.DistributionNumber;
            dividends = item.Dividends;
            individualHtmlText = item.IndividualHtmlText;
            individualHtmlText2 = item.individualHtmlText2;
            interests = item.Interests;
            investorCommitmentId = item.InvestorCommitmentId;
            investorExpenses = item.InvestorExpenses;
            lookBackInterests = item.LookBackInterests;
            openCommitment = item.OpenCommitment;
            otherIncome = item.OtherIncome;
            partnershipExpenses = item.PartnershipExpenses;
            recallableAmount = item.RecallableAmount;
            reportDate = item.ReportDate;
            returnOfCapital = item.ReturnOfCapital;
            totalContribution = item.TotalContribution;
            totalContributionCommitment = item.TotalContributionCommitment;
            totalDistribution = item.TotalDistribution;
            totalDistributionCommitment = item.TotalDistribution;
            totalDistributionPaidIn = item.TotalDistributionPaidIn;
            withholdingTax = item.WithholdingTax;
        }
    }
}

