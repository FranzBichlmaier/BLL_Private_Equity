using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace BLL_Private_Equity.Berechnungen
{
    public class AnalyzingParameter: BindableBase
    {
        private AnalyzingParameterPeriod period = AnalyzingParameterPeriod.Auto;

        public AnalyzingParameterPeriod Period
        {
            get { return this.period; }
            set { SetProperty(ref period, value); }
        }


        private DateTime startDate = DateTime.MinValue;

        public DateTime StartDate
        {
            get { return this.startDate; }
            set { SetProperty(ref startDate, value); }
        }

        private DateTime endDate=DateTime.MaxValue;

        public DateTime EndDate
        {
            get { return this.endDate; }
            set { SetProperty(ref endDate, value); }
        }

        private List<ExtendedCommitment> commitments;

        public List<ExtendedCommitment> Commitments
        {
            get { return this.commitments; }
            set { SetProperty(ref commitments, value); }
        }

        public AnalyzingParameter()
        {
            Commitments = new List<ExtendedCommitment>();
        }
    
    }
}
