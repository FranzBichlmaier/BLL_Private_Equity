using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL_DataModels;

namespace BLL_Private_Equity.Comparer
{
    public class CashflowComparer : IComparer<InvestorCashFlow>
    {
        public int Compare(InvestorCashFlow x, InvestorCashFlow y)
        {
            if (x.EffectiveDate > y.EffectiveDate) return 1;
            if (x.EffectiveDate < y.EffectiveDate) return -1;
            return 0;
        }
    }
}
