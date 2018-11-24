using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Berechnungen
{
    public class CashFlowErrors: BindableBase
    {

        private int peFundId;

        public int PeFundId
        {
            get { return this.peFundId; }
            set { SetProperty(ref peFundId, value); }
        }

        private int investorId;

        public int InvestorId
        {
            get { return this.investorId; }
            set { SetProperty(ref investorId, value); }
        }

        private string objectName;

        public string ObjectName
        {
            get { return this.objectName; }
            set { SetProperty(ref objectName, value); }
        }

        private string errorText;

        public string ErrorText
        {
            get { return this.errorText; }
            set { SetProperty(ref errorText, value); }
        }
    }
}
