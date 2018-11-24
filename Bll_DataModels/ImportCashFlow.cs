using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_DataModels
{
    public class ImportCashFlow
    {
        public string PeFundNumber { get; set; }
        public string PeFundName { get; set; }
        public int PeFundId { get; set; }
        public string InvestorNumber { get; set; }
        public int InvestorId { get; set; }
        public int InvestorCommitmentId { get; set; }
        public long PsPlusId { get; set; }
        public DateTime CashFlowDate { get; set; }
        public DateTime InputDate { get; set; }
        public double AmountPeFundCurrency { get; set; }
        public double AmountInvestorCurrency { get; set; }
        public double CapitalGain { get; set; }
        public double ReturnOfCapital { get; set; }
        public double Dividends { get; set; }
        public double WithholdingTax { get; set; }
        public double PartnershipExpenses { get; set; }
        public double RecallableAmount { get; set; }
        public double ExchangeRate { get; set; }
    }
}
