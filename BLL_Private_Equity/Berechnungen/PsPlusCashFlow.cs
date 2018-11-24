using Prism.Mvvm;
using BLL_DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Berechnungen
{
    public class PsPlusCashFlow: BindableBase
    {

        private InvestorCashFlow investorCashFlow;

        public InvestorCashFlow InvestorCashFlow
        {
            get { return this.investorCashFlow; }
            set { SetProperty(ref investorCashFlow, value); }
        }

        private string investorReference;

        public string InvestorReference
        {
            get { return this.investorReference; }
            set { SetProperty(ref investorReference, value); }
        }

        private int investorId;

        public int InvestorId
        {
            get { return this.investorId; }
            set { SetProperty(ref investorId, value); }
        }

        private string investorHqTrustAccount= string.Empty;

        public string InvestorHqTrustAccount
        {
            get { return this.investorHqTrustAccount; }
            set { SetProperty(ref investorHqTrustAccount, value); }
        }
    
    }
}
