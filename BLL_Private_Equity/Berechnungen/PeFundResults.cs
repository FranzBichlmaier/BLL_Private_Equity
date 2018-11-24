using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DateTimeFunctions;
using Excel.FinancialFunctions;
using Prism.Mvvm;
using BLL_DataModels;
using BLL_DataAccess;

namespace BLL_Private_Equity.Berechnungen
{
    public class PeFundResults : BindableBase
    {
        private double commitmentAmount;
        private double amountCalled;
        private double amountCalledInvestorCurrency;
        private double amountCalledInPercentOfCommitment;
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
        private double totalValue;

        private DateDifferences dateFunctions = new DateDifferences();
        
        private ObservableCollection<InvestorCommitment> commitments = new ObservableCollection<InvestorCommitment>();
        private ObservableCollection<PeFundPerformance> fundPerformances = new ObservableCollection<PeFundPerformance>();
        private DateTime dateFrom = new DateTime(1990, 1, 1);
        private DateTime dateTo = new DateTime(2100, 12, 31);

        private List<DateTime> cfDates = new List<DateTime>();
        private List<Double> cfAmounts = new List<double>();
        private List<Double> cfAmountsInvestorCurrency = new List<double>();

        private List<CashFlowLight> cfCollection = new List<CashFlowLight>();


        public PeFundResults(List<InvestorCommitment> myCommitments, DateTime? mydateFrom, DateTime? mydateTo)
        {
            commitments.Clear();
            if (mydateFrom != null) this.dateFrom = (DateTime)mydateFrom;
            if (mydateTo != null) this.dateTo = (DateTime)mydateTo; else this.dateTo = dateFunctions.PreviousQuarter(DateTime.Now);
            foreach(InvestorCommitment commitment in myCommitments)
            {
                
                commitments.Add(commitment);
            }
            CalculateResults();
        }

        public PeFundResults(InvestorCommitment myCommitment, DateTime? mydateFrom, DateTime? mydateTo)
        {
            commitments.Clear();
            if (mydateFrom != null) this.dateFrom = (DateTime)mydateFrom;
            if (mydateTo != null) this.dateTo = (DateTime)mydateTo; else this.dateTo = dateFunctions.PreviousQuarter(DateTime.Now);
            commitments.Add(myCommitment);
            CalculateResults();
        }

        private void CalculateResults()
        {
            ClearFields();

            foreach(InvestorCommitment commitment in commitments)
            {
                if (commitment.InvestorCashFlows.Count == 0) continue;
                try
                {
                    PeFundPerformance performance = new PeFundPerformance(commitment, dateFrom, dateTo);        // the individual Performance of each PeFund is added to a collection
                    performance.InvestorCommitmentId = commitment.Id;
                    fundPerformances.Add(performance);
                }
                catch (Exception)
                {
                    continue;
                }
               

                foreach (InvestorCashFlow cashFlow in commitment.InvestorCashFlows)
                {
                    if (cashFlow.EffectiveDate > dateTo) continue;
                    if (cashFlow.EffectiveDate < dateFrom) continue;

                    cfCollection.Add(new CashFlowLight()
                    {
                        CDate = cashFlow.EffectiveDate,
                        CAmount = cashFlow.CashFlowAmount,
                        CAmountInvestorCurrency = cashFlow.CashFlowAmountInInvestorCurrency
                    }
                        );
                   

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
                            amountCalled -= cashFlow.RecallableAmount;
                            amountCalledInvestorCurrency -= Math.Round(cashFlow.RecallableAmount / cashFlow.CashFlowAmount * cashFlow.CashFlowAmountInInvestorCurrency, 2);
                        }
                    }
                }
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

                //InvestorPcap pcap = commitment.InvestorPcaps.Find(p => p.AsOfDate == dateTo);
                //if (pcap == null)
                //{
                //    //throw new Exception($"der NAV für Fund {commitment.PeFund.FundName} und Datum {dateTo:d} ist nicht vorhanden");
                //    pcap = commitment.InvestorPcaps.Find(p => p.AsOfDate > dateTo);
                //    if (pcap == null)
                //    {
                //        pcap = commitment.InvestorPcaps.Find(p => p.AsOfDate < dateTo);
                //        if (pcap == null)
                //        {
                //            throw new Exception($"der NAV für Fund {commitment.PeFund.FundName} und Datum {dateTo:d} ist nicht vorhanden");
                //        }
                //    }
                //}
                cfCollection.Add(new CashFlowLight()
                {
                    CDate = dateTo,
                    CAmount = pcap.FinalPcapAmount,
                    CAmountInvestorCurrency = pcap.FinalPcapAmountInInvestorCurrency
                }
                 );
           
                valuationFundCurrency += pcap.FinalPcapAmount;
                valuationInvestorCurrency += pcap.FinalPcapAmountInInvestorCurrency;
                commitmentAmount += commitment.CommitmentAmount;
                openCommitment = commitmentAmount + amountCalled;
                amountCalledInPercentOfCommitment = Math.Abs(Math.Round(amountCalled / commitmentAmount, 4));
            }
            tvpi = Math.Round((amountDistributed + valuationFundCurrency) / amountCalled, 2);
            dpi = Math.Round(amountDistributed / amountCalled, 2);
            rvpi = Math.Round(valuationFundCurrency / amountCalled, 2);
            tvpi = Math.Abs(tvpi);
            rvpi = Math.Abs(rvpi);
            dpi = Math.Abs(dpi);
            totalValue = Math.Round(amountDistributed + valuationFundCurrency, 2);

            Comparer.CashFlowLightComparer cashFlowLightComparer = new Comparer.CashFlowLightComparer();
            cfCollection.Sort(cashFlowLightComparer);

            foreach(CashFlowLight light in cfCollection)
            {
                cfDates.Add(light.CDate);
                cfAmounts.Add(light.CAmount);
                cfAmountsInvestorCurrency.Add(light.CAmountInvestorCurrency);
            }

            if (cfCollection.Count>1)
            {
                try
                {
                    irr = Financial.XIrr(cfAmounts, cfDates) * 100;
                    irrInvestorCurrency = Financial.XIrr(cfAmountsInvestorCurrency, cfDates) * 100;
                }
                catch (Exception )
                {
                    irr = 0;
                    irrInvestorCurrency = 0;
                }
          
            }
            else
            {
                irr = 0;
                irrInvestorCurrency = 0;
            }
        }

        private void ClearFields()
        {
              commitmentAmount=0;
              amountCalled=0;
              openCommitment=0;
              amountDistributed=0;
              valuationFundCurrency=0;
              valuationInvestorCurrency=0;
              tvpi=0;
              rvpi=0;
              dpi=0;
              irr=0;
            totalValue = 0;
        }

        public double Irr { get => irr; set => SetProperty(ref irr, value); }
        public double Dpi { get => dpi; set => SetProperty(ref dpi, value); }
        public double Rvpi { get => rvpi; set => SetProperty(ref rvpi, value); }
        public double Tvpi { get => tvpi; set => SetProperty(ref tvpi, value); }
        public double ValuationFundCurrency { get => valuationFundCurrency; set => SetProperty(ref valuationFundCurrency, value); }
        public double AmountDistributed { get => amountDistributed; set => SetProperty(ref amountDistributed, value); }
        public double OpenCommitment { get => openCommitment; set => SetProperty(ref openCommitment, value); }
        public double AmountCalled { get => amountCalled; set => SetProperty(ref amountCalled, value); }
        public double CommitmentAmount { get => commitmentAmount; set => SetProperty(ref commitmentAmount, value); }
        public double ValuationInvestorCurrency { get => valuationInvestorCurrency; set => SetProperty(ref valuationInvestorCurrency, value); }
        public double AmountCalledInvestorCurrency { get => amountCalledInvestorCurrency; set => SetProperty(ref amountCalledInvestorCurrency, value); }
        public double AmountDistributedInvestorCurrency { get => amountDistributedInvestorCurrency; set => SetProperty(ref amountDistributedInvestorCurrency, value); }
        public double TvpiInvestorCurrency { get => tvpiInvestorCurrency; set => SetProperty(ref tvpiInvestorCurrency, value); }
        public double RvpiInvestorCurrency { get => rvpiInvestorCurrency; set => SetProperty(ref rvpiInvestorCurrency, value); }
        public double DpiInvestorCurrency { get => dpiInvestorCurrency; set => SetProperty(ref dpiInvestorCurrency, value); }
        public double IrrInvestorCurrency { get => irrInvestorCurrency; set => SetProperty(ref irrInvestorCurrency, value); }
        public double TotalValue { get => totalValue; set => SetProperty(ref totalValue, value); }
        public double AmountCalledInPercentOfCommitment { get => amountCalledInPercentOfCommitment; set => SetProperty(ref amountCalledInPercentOfCommitment, value); }
        public ObservableCollection<PeFundPerformance> FundPerformances { get => fundPerformances; set => SetProperty(ref fundPerformances,value); }
    }
}
