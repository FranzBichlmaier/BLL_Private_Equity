using BLL_DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Comparer
{
    public class InvestorFullNameComparer : IComparer<Investor>
    {
        public int Compare(Investor x, Investor y)
        {
            return string.Compare(x.IName.FullName, y.IName.FullName);
        }
    }
}
