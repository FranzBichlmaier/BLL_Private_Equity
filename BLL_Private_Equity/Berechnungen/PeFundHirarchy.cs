using BLL_DataAccess;
using BLL_DataModels;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Berechnungen
{
    public class PeFundHirarchy: BindableBase
    {

        private PeFund fund;

        public PeFund Fund
        {
            get { return this.fund; }
            set { SetProperty(ref fund, value); }
        }
        private ObservableCollection<PeFundHirarchy> feeder = new ObservableCollection<PeFundHirarchy>();

        private ObservableCollection<InvestorCommitment> commitments;

        public ObservableCollection<InvestorCommitment> Commitments
        {
            get { return this.commitments; }
            set { SetProperty(ref commitments, value); }
        }
        private int nrOfInvestors = 0;
        private double totalCommitments = 0;
        private bool isExpanded = true;

        
        public ObservableCollection<PeFundHirarchy> Feeder { get => feeder; set => SetProperty(ref feeder, value); }
       
        public int NrOfInvestors { get => nrOfInvestors; set => SetProperty(ref nrOfInvestors, value); }
        public double TotalCommitments { get => totalCommitments; set => SetProperty(ref totalCommitments, value); }
        public bool IsExpanded { get => isExpanded; set => SetProperty(ref isExpanded, value); }


        public PeFundHirarchy()
        {

        }

        public PeFundHirarchy(PeFund fund)
        {
            this.fund = GetTopFund(fund.Id);
            this.commitments = new ObservableCollection<InvestorCommitment>( PefundAccess.GetCommitmentsForPeFund(this.fund.Id));
            nrOfInvestors = commitments.Count;
            foreach (InvestorCommitment comm in commitments)
            {
                totalCommitments += comm.CommitmentAmount;
            }
            foreach (PeFund f in this.fund.FeederFunds)
            {
                feeder.Add(SetFeederFund(f.Id));
            }
        }

        private PeFundHirarchy SetFeederFund(int id)
        {
            PeFundHirarchy hirarchy = new PeFundHirarchy();
            hirarchy.Fund = PefundAccess.GetPeFundById(id);
            hirarchy.Commitments = new ObservableCollection<InvestorCommitment>(PefundAccess.GetCommitmentsForPeFund(id));
            hirarchy.NrOfInvestors = hirarchy.Commitments.Count;
            foreach (InvestorCommitment comm in hirarchy.Commitments)
            {
                hirarchy.TotalCommitments += comm.CommitmentAmount;
            }

            foreach (PeFund lowerLevelFund in hirarchy.Fund.FeederFunds)
            {
                hirarchy.Feeder.Add(SetFeederFund(lowerLevelFund.Id));
            }

            return hirarchy;
        }

  

        private PeFund GetTopFund(int id)
        {
            PeFund f = new PeFund();
            f.PeFundId = id;

            while (f.PeFundId != null && f.PeFundId != 0)
            {
                f = PefundAccess.GetPeFundById((int)f.PeFundId);
            }
            return f;
        }
    }
}
