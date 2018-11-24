using Excel.FinancialFunctions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using BLL_DataModels;
using BLL_DataAccess;

namespace BLL_Private_Equity.Berechnungen
{
    public class PeFundPerformance
    {
        private int id;
        private int investorCommitmentId;
        private int reportId;
        private DateTime asOfDate;
        private TimeSpan duration;
        private string peFundName;
        private string currencyName;
        private double commitmentAmount;
        private double amountCalled;
        private double amountCalledInvestorCurrency;
        private double openCommitment;
        private double amountDistributed;
        private double amountDistributedInvestorCurrency;
        private double valuationFundCurrency;
        private double valuationInvestorCurrency;
        private double tvpi;
        private double tvpiInvestorCurrency;
        private double rvpi;
        private double rvpiInvestorCurrency;
        private double dpi;
        private double dpiInvestorCurrency;
        private double irr;
        private double irrInvestorCurrency;
        private PeFund peFund;
        private double totalValue;

        private List<DateTime> cfDates = new List<DateTime>();
        private List<double> cfAmounts = new List<double>();
        private List<double> cfAmountsInvestorCurrency = new List<double>();

        public int Id { get => id; set => id=value; }
        public DateTime AsOfDate { get => asOfDate; set => asOfDate = value; }
        public TimeSpan Duration { get => duration; set => duration = value; }
        public int InvestorCommitmentId { get=> investorCommitmentId; set => investorCommitmentId=value; }
        public int ReportId { get => reportId; set => reportId = value; }
        public string PeFundName { get => peFundName; set => peFundName = value; }
        public string CurrencyName { get => currencyName; set => currencyName = value; }
        public double CommitmentAmount { get => commitmentAmount; set => commitmentAmount = value; }
        public double AmountCalled { get => amountCalled; set => amountCalled = value; }
        public double AmountCalledInvestorCurrency { get => amountCalledInvestorCurrency; set => amountCalledInvestorCurrency = value; }
        public double OpenCommitment { get => openCommitment; set => openCommitment = value; }
        public double AmountDistributed { get => amountDistributed; set => amountDistributed = value; }
        public double AmountDistributedInvestorCurrency { get => amountDistributedInvestorCurrency; set => amountDistributedInvestorCurrency = value; }
        public double ValuationFundCurrency { get => valuationFundCurrency; set => valuationFundCurrency = value; }
        public double ValuationInvestorCurrency { get => valuationInvestorCurrency; set => valuationInvestorCurrency = value; }
        public double Tvpi { get => tvpi; set => tvpi = value; }
        public double TvpiInvestorCurrency { get => tvpiInvestorCurrency; set => tvpiInvestorCurrency = value; }
        public double Rvpi { get => rvpi; set => rvpi = value; }
        public double RvpiInvestorCurrency { get => rvpiInvestorCurrency; set => rvpiInvestorCurrency = value; }
        public double Dpi { get => dpi; set => dpi = value; }
        public double DpiInvestorCurrency { get => dpiInvestorCurrency; set => dpiInvestorCurrency = value; }
        public double Irr { get => irr; set => irr = value; }
        public double IrrInvestorCurrency { get => irrInvestorCurrency; set => irrInvestorCurrency = value; }
        [NotMapped]
        public PeFund PeFund { get => peFund; set => peFund = value; }
        public double TotalValue { get => totalValue; set => totalValue = value; }

        public PeFundPerformance()
        {

        }

        public PeFundPerformance(InvestorCommitment commitment, DateTime dateFrom, DateTime dateTo)
        {
            foreach (InvestorCashFlow cashFlow in commitment.InvestorCashFlows)
            {
                if (cashFlow.EffectiveDate > dateTo) continue;
                if (cashFlow.EffectiveDate < dateFrom) continue;
                cfDates.Add(cashFlow.EffectiveDate);
                cfAmounts.Add(cashFlow.CashFlowAmount);
                cfAmountsInvestorCurrency.Add(cashFlow.CashFlowAmountInInvestorCurrency);

                if (cashFlow.CashFlowType == "Capital Call")
                {
                    amountCalled += cashFlow.CashFlowAmount;
                    amountCalledInvestorCurrency += cashFlow.CashFlowAmountInInvestorCurrency;
                }
                else if (cashFlow.CashFlowType == "Distribution")
                {
                    amountDistributed += cashFlow.CashFlowAmount;
                    amountDistributedInvestorCurrency += cashFlow.CashFlowAmountInInvestorCurrency;

                    if (cashFlow.RecallableAmount != 0)
                    {
                        amountDistributed -= cashFlow.RecallableAmount;
                        amountDistributedInvestorCurrency -= Math.Round(cashFlow.RecallableAmount / cashFlow.CashFlowAmount * cashFlow.CashFlowAmountInInvestorCurrency, 2);
                        amountCalled += cashFlow.RecallableAmount;
                        amountCalledInvestorCurrency += Math.Round(cashFlow.RecallableAmount / cashFlow.CashFlowAmount * cashFlow.CashFlowAmountInInvestorCurrency, 2);
                    }
                }
            }

            peFund = commitment.PeFund;
            // use market value (of DateTo)  als last distribution

            InvestorPcap pcap = PefundAccess.GetLastPCap(commitment, dateTo);
            if (pcap == null)
            {
                pcap = new InvestorPcap()
                {
                    InvestorCommitmentId = commitment.Id,
                    AsOfDate = DateTime.MinValue,
                    FinalPcapAmount = 0
                };
            }
            pcap.FinalPcapAmount = PefundAccess.NavCalculation(pcap, dateTo);
            

            if (pcap != null && pcap.FinalPcapAmount != 0 && commitment.InvestorCashFlows.Count > 0)
            {
                DateTime minDate = cfDates.Min(i => i.Date);
                Duration = dateTo.Subtract(minDate);

                cfDates.Add(dateTo);
                cfAmounts.Add(pcap.FinalPcapAmount);
                cfAmountsInvestorCurrency.Add(pcap.FinalPcapAmountInInvestorCurrency);
                valuationFundCurrency += pcap.FinalPcapAmount;
                valuationInvestorCurrency += pcap.FinalPcapAmountInInvestorCurrency;
                commitmentAmount += commitment.CommitmentAmount;
                openCommitment = commitmentAmount + amountCalled;
                PeFundName = commitment.PeFund.FundName;
                CurrencyName = commitment.PeFund.Currency.CurrencyName;
                totalValue = Math.Round(amountDistributed + valuationFundCurrency, 2);

                tvpi = Math.Round((amountDistributed + valuationFundCurrency) / amountCalled, 2);
                dpi = Math.Round(amountDistributed / amountCalled, 2);
                rvpi = Math.Round(valuationFundCurrency / amountCalled, 2);
                tvpi = Math.Abs(tvpi);
                rvpi = Math.Abs(rvpi);
                dpi = Math.Abs(dpi);
                try
                {
                    irr = Financial.XIrr(cfAmounts, cfDates) * 100;
                    if (pcap.FinalPcapAmountInInvestorCurrency>0) irrInvestorCurrency = Financial.XIrr(cfAmountsInvestorCurrency, cfDates) * 100;
                }
                catch (Exception ex)
                {
                    irr = 0;
                    tvpi = 0;
                    dpi = 0;
                    rvpi = 0;
                }
            }
            else
            {
                openCommitment = commitmentAmount;
                PeFundName = commitment.PeFund.FundName;
                CurrencyName = commitment.PeFund.Currency.CurrencyName;
            }
           
        }
    }
}
