using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Berechnungen
{
    public class ExtendedPcap: BindableBase
    {
        private DateTime asOfDate;

        public DateTime AsOfDate

        {
            get { return this.asOfDate; }
            set { SetProperty(ref asOfDate, value); }
        }

        private double navAmount;

        public double NavAmount
        {
            get { return this.navAmount; }
            set { SetProperty(ref navAmount, value); }
        }

        private double navAmountInInvestorCurrency;

        public double NavAmountInInvestorCurrency
        {
            get { return this.navAmountInInvestorCurrency; }
            set { SetProperty(ref navAmountInInvestorCurrency, value); }
        }

        private double callsInPeriod;

        public double CallsInPeriod
        {
            get { return this.callsInPeriod; }
            set { SetProperty(ref callsInPeriod, value); }
        }

        private double distributionsInPeriod;

        public double DistributionsInPeriod
        {
            get { return this.distributionsInPeriod; }
            set { SetProperty(ref distributionsInPeriod, value); }
        }

        private double callsSinceInception;

        public double CallsSinceInception
        {
            get { return this.callsSinceInception; }
            set { SetProperty(ref callsSinceInception, value); }
        }

        private double distributionsSinceInception;

        public double DistributionsSinceInception
        {
            get { return this.distributionsSinceInception; }
            set { SetProperty(ref distributionsSinceInception, value); }
        }

        private double profitLossInQuarter;

        public double ProfitLossInQuarter
        {
            get { return this.profitLossInQuarter; }
            set { SetProperty(ref profitLossInQuarter, value); }
        }
    }
}
