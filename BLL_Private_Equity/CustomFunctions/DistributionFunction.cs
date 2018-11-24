using BLL_DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Data;

namespace BLL_Private_Equity.CustomFunctions
{
    public class DistributionFunction: AggregateFunction<InvestorCashFlow, double>
    {
        public DistributionFunction()
        {
            this.AggregationExpression = items => Distributions(items);
        }

        private double Distributions(IEnumerable<InvestorCashFlow> source)
        {
            var values = source.Where(i => i.CashFlowAmount > 0).Select(i => i.CashFlowAmount);
            return values.Sum();
        }
    }
}
