﻿using BLL_DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Comparer
{
    public class InvestorCashFlowComparer : IComparer<InvestorCashFlow>
    {
        public int Compare(InvestorCashFlow x, InvestorCashFlow y)
        {
            if (x.EffectiveDate == y.EffectiveDate) return 0;
            if (x.EffectiveDate > y.EffectiveDate) return 1;
            return -1;
        }
    }
}
