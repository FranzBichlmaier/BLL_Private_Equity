using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_DataModels
{
    public class CashFlowBarChart
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }    //Quarter Year or Month year or year
        public double Calls { get; set; }
        public double Distributions { get; set; }
        public double Balance { get; set; }
        public double CumulatedBalance { get; set; }
        public double CumulatedCapitalCalls  { get; set; }
        public double CumulatedDistributions { get; set; }
        public double Nav { get; set; }
        public int InvestorCommitmentId { get; set; }
        public int ReportId { get; set; }
        public DateTime ReportAsOf { get; set; }
    }
}
