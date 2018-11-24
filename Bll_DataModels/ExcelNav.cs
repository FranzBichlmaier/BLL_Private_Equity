using System;

namespace BLL_DataModels
{
    public class ExcelNav
    {
        public string FundWkn { get; set; }
        public string InvestorNumber { get; set; }
        public DateTime NavDate { get; set; }
        public double NavAmount { get; set; }
        public int InvestorId { get; set; }
        public int PeFundId { get; set; }
        public int InvestorCommitmentId { get; set; }
    }
}
