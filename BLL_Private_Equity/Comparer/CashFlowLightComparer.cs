using BLL_Private_Equity.Berechnungen;
using System.Collections.Generic;

namespace BLL_Private_Equity.Comparer

{
    public class CashFlowLightComparer : IComparer<CashFlowLight>
    {
        public int Compare(CashFlowLight x, CashFlowLight y)
        {
            if (x.CDate > y.CDate) return 1;
            if (x.CDate < y.CDate) return -1;
            return 0;
        }
    }
}
