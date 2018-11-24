using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Berechnungen
{
    public class BubbleScatterChart: BindableBase
    {

        private double irr;

        public double Irr
        {
            get { return this.irr; }
            set { SetProperty(ref irr, value); }
        }

        private double tvpi;

        public double Tvpi
        {
            get { return this.tvpi; }
            set { SetProperty(ref tvpi, value); }
        }

        private double bubbleSize;

        public double BubbleSize
        {
            get { return this.bubbleSize; }
            set { SetProperty(ref bubbleSize, value); }
        }

        private string legendText;

        public string LegendText
        {
            get { return this.legendText; }
            set { SetProperty(ref legendText, value); }
        }

        private string label;

        public string Label
        {
            get { return this.label; }
            set { SetProperty(ref label, value); }
        }

        private PeFundPerformance fundPerformance;

        public PeFundPerformance FundPerformance
        {
            get { return this.fundPerformance; }
            set { SetProperty(ref fundPerformance, value); }
        } 
    }
}
