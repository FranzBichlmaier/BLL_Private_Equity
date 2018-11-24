using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL_DataModels;

namespace BLL_Private_Equity.Comparer
{
    public class PcapComparer : IComparer<InvestorPcap>
    {
        public int Compare(InvestorPcap x, InvestorPcap y)
        {
            if (x.AsOfDate > y.AsOfDate) return 1;
            if (x.AsOfDate < y.AsOfDate) return -1;
            return 0;
        }
    }
}
