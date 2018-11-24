using BLL_DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Data;

namespace BLL_Private_Equity.CustomFunctions
{
    public class CapitalCallFunction : AggregateFunction<InvestorCashFlow, double>
    {
        public CapitalCallFunction()
        {
            this.AggregationExpression = items => Calls(items);
        }
        private double Calls(IEnumerable<InvestorCashFlow> source)
        {
            var values = source.Where(i => i.CashFlowAmount < 0).Select(i => i.CashFlowAmount);
            return values.Sum();
        }
    }
}
