using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_DataModels
{
    public class UniqueCashFlowNumber
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private DateTime cashFlowDate;

        public DateTime CashFlowDate
        {
            get { return cashFlowDate; }
            set { cashFlowDate = value; }
        }
        private int? peFundId;

        public int? PeFundId
        {
            get { return peFundId; }
            set { peFundId = value; }
        }
        private double cashFlowAmount;
        [NotMapped]
        public double CashFlowAmount
        {
            get { return cashFlowAmount; }
            set { cashFlowAmount = value; }
        }


        private string cashFlowType;
        [NotMapped]
        public string CashFlowType
        {
            get { return cashFlowType; }
            set { cashFlowType = value; }
        }
        private List<InvestorCashFlow> investorCashFlows;
        [NotMapped]
        public List<InvestorCashFlow> InvestorCashFlows
        {
            get { return investorCashFlows; }
            set { investorCashFlows = value; }
        }



    }
}
