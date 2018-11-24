using System;

namespace BLL_DataModels
{
    public class PrintPcap
    {
        public int Id { get; set; }
        public string PeFundName { get; set; }
        public int PeFundId { get; set; }
        public double FundCommitment { get; set; }
        public DateTime YearEnd { get; set; }
        public DateTime LastQuarterEnd { get; set; }
        public DateTime CurrentQuarterEnd { get; set; }
        public double FundPcapYearEnd { get; set; }
        public double FundPcapLastQuarter { get; set; }
        public double FundPcapCurrentQuarter { get; set; }
        public string InvestorName { get; set; }
        public int InvestorId { get; set; }
        public double InvestorCommitment { get; set; }
        public double InvestorPcapYearEnd { get; set; }
        public double InvestorPcapLastQuarter { get; set; }
        public double InvestorPcapCurrentQuarter { get; set; }
        public double  FundContributionsInception { get; set; }
        public double FundContributionsYearEnd { get; set; }
        public double FundContributionsCurrentQuarter { get; set; }
        public double FundResultYearEnd { get; set; }
        public double FundResultQuarter { get; set; }
        public double InvestorContributionsInception { get; set; }
        public double InvestorContributionsYearEnd { get; set; }
        public double InvestorContributionsCurrentQuarter { get; set; }
        public double InvestorResultYearEnd { get; set; }
        public double InvestorResultQuarter { get; set; }
        public PeFundCashFlow FundDistributionsYearEnd { get; set; }
        public InvestorCashFlow InvestorDistributionsYearEnd { get; set; }
    }
}
