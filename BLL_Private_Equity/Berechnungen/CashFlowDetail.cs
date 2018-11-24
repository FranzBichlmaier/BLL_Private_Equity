using BLL_DataModels;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Berechnungen
{
    public class CashFlowDetail:BindableBase
    {
        private Investor investor;
        private int investorId;
        private int investorCommitmentId;
        private string reference;
        private string secondSignature;
        private double cashFlowAmountCall;
        private double cashFlowAmountDistribution;
        private double commitmentAmount;
        private double cashFlowAmount;
        private double cashFlowAmountInInvestorCurrency;
        private double returnOfCapital;
        private double capitalGain;
        private double interests;
        private double dividends;
        private double otherIncome;
        private double recallableAmount;
        private double partnershipExpenses;
        private double withholdingTax;
        private double investorExpenses;
        private double lookbackInterests;
        private double openCommitment;
        private double openCommitmentInPercent;
        private double totalCalls;
        private double totalDistributions;
        private double totalCallsInPercent;
        private double totalDistributionsInPercent;
        private double openPosition;
        private BankAccount bankAccount;
        private string fileName;

        public Investor Investor { get => investor; set => SetProperty(ref investor, value); }
        public int InvestorId { get => investorId; set => SetProperty(ref investorId, value); }
        public int InvestorCommitmentId { get => investorCommitmentId; set => SetProperty(ref investorCommitmentId, value); }
        public string Reference { get => reference; set => SetProperty(ref reference, value); }
       
        public string SecondSignature { get => secondSignature; set => SetProperty(ref secondSignature, value); }
        
        public double CommitmentAmount { get => commitmentAmount; set => SetProperty(ref commitmentAmount, value); }
        public double CashFlowAmountCall { get => cashFlowAmountCall; set => SetProperty(ref cashFlowAmountCall, value); }
        public double CashFlowAmountDistribution { get => cashFlowAmountDistribution; set => SetProperty(ref cashFlowAmountDistribution, value); }
        public double CashFlowAmount { get => cashFlowAmount; set => SetProperty(ref cashFlowAmount, value); }
        public double CashFlowAmountInInvestorCurrency { get => cashFlowAmountInInvestorCurrency; set => SetProperty(ref cashFlowAmountInInvestorCurrency, value); }
        public double ReturnOfCapital { get => returnOfCapital; set => SetProperty(ref returnOfCapital, value); }
        public double CapitalGain { get => capitalGain; set => SetProperty(ref capitalGain, value); }
        public double Interests { get => interests; set => SetProperty(ref interests, value); }
        public double Dividends { get => dividends; set => SetProperty(ref dividends, value); }
        public double OtherIncome { get => otherIncome; set => SetProperty(ref otherIncome, value); }
        public double RecallableAmount { get => recallableAmount; set => SetProperty(ref recallableAmount, value); }
        public double PartnershipExpenses { get => partnershipExpenses; set => SetProperty(ref partnershipExpenses, value); }
        public double WithholdingTax { get => withholdingTax; set => SetProperty(ref withholdingTax, value); }
        public double InvestorExpenses { get => investorExpenses; set => SetProperty(ref investorExpenses, value); }
        public double LookbackInterests { get => lookbackInterests; set => SetProperty(ref lookbackInterests, value); }
        public double OpenPosition { get => openPosition; set => SetProperty(ref openPosition, value); }
        public BankAccount BankAccount { get => bankAccount; set => SetProperty(ref bankAccount, value); }
        public double OpenCommitment { get => openCommitment; set => SetProperty(ref openCommitment, value); }
        public double OpenCommitmentPercent { get => openCommitment; set => SetProperty(ref openCommitmentInPercent, value); }
        public double TotalCalls { get => totalCalls; set => SetProperty(ref totalCalls, value); }
        public double TotalDistributions { get => totalDistributions; set => SetProperty(ref totalDistributions, value); }
        public double TotalCallsInPercent { get => totalCallsInPercent; set => SetProperty(ref totalCallsInPercent, value); }
        public double TotalDistributionsInPercent { get => totalDistributionsInPercent; set => SetProperty(ref totalDistributionsInPercent, value); }
        public string FileName { get => fileName; set => SetProperty(ref fileName, value); }
    }
}
