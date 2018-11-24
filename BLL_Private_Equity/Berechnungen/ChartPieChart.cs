using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Berechnungen
{
    public class ChartPieChart: BindableBase
    {

        private string label;

        public string Label
        {
            get { return this.label; }
            set { SetProperty(ref label, value); }
        }

        private double amount;

        public double Amount
        {
            get { return this.amount; }
            set { SetProperty(ref amount, value); }
        }

        private string legendText;

        public string LegendText
        {
            get { return this.legendText; }
            set { SetProperty(ref legendText, value); }
        }

        private PeFundPerformance fundPerformance;

        public PeFundPerformance FundPerformance
        {
            get { return this.fundPerformance; }
            set { SetProperty(ref fundPerformance, value); }
        }
    }
}
