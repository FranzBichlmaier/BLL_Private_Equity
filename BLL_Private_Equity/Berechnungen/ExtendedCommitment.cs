using BLL_DataModels;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Berechnungen
{
    public class ExtendedCommitment: BindableBase
    {
        private InvestorCommitment   commitment;

        public InvestorCommitment Commitment
        {
            get { return this.commitment; }
            set { SetProperty(ref commitment, value); }
        }

        private List<ExtendedPcap> extendedPcaps;

        public List<ExtendedPcap> ExtendedPcaps
        {
            get { return this.extendedPcaps; }
            set { SetProperty(ref extendedPcaps, value); }
        }
        public ExtendedCommitment()
        {
            extendedPcaps = new List<ExtendedPcap>();
        }
    }
}
