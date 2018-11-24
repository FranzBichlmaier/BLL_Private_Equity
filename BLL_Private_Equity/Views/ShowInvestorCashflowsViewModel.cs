using BLL_Prism;
using Prism.Regions;
using BLL_DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BLL_DataAccess;
using System.Windows.Data;
using BLL_Private_Equity.Berechnungen;
using System.Windows.Input;
using Prism.Commands;

namespace BLL_Private_Equity.Views
{
    public class ShowInvestorCashFlowsViewModel: HqtBindableBase
    {

        private InvestorAccess investorAccess = new InvestorAccess();
        private InvestorCommitment commitment;

        private string headline;

        public string Headline
        {
            get { return this.headline; }
            set { SetProperty(ref headline, value); }
        }

        private PeFundResults fundResults;

        public PeFundResults FundResults
        {
            get { return this.fundResults; }
            set { SetProperty(ref fundResults, value); }
        }

        public InvestorCommitment Commitment
        {
            get { return this.commitment; }
            set { SetProperty(ref commitment, value); }
        }
        public ICommand GridViewSelectionChangedCommand { get; set; }
        private ObservableCollection<InvestorCashFlow> cashflows = new ObservableCollection<InvestorCashFlow>();
        public ICollectionView Cashflows { get; set; }
        public ShowInvestorCashFlowsViewModel()
        {
            GridViewSelectionChangedCommand = new DelegateCommand<object>(OnGridViewSelectionChanged);
        }

        private void OnGridViewSelectionChanged(object obj)
        {
            
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Commitment = navigationContext.Parameters["Commitment"] as InvestorCommitment;
            // add additionalInformation about Investor and Fund
            Commitment = investorAccess.GetFullyLoadedInvestorCommitment(commitment.Id);
            if (Commitment != null)
            {
                cashflows = new ObservableCollection<InvestorCashFlow>(investorAccess.GetCashFlowsForCommitment(Commitment.Id));
                Cashflows = CollectionViewSource.GetDefaultView(cashflows);
                FundResults = new PeFundResults(Commitment, null, null);
                RaisePropertyChanged("Cashflows");

                // Set tabtitle

                if (string.IsNullOrEmpty(Commitment.Investor.InvestorHqTrustAccount))
                    TabTitle = Commitment.Investor.InvestorReference + " (Cashflows)";
                else TabTitle = Commitment.Investor.InvestorHqTrustAccount + " (Cashflows)";

                // set headline

                StringBuilder builder = new StringBuilder(Commitment.PeFund.FundName + " ");
                builder.Append("  Commitment: ");
                builder.Append(Commitment.PeFund.Currency.CurrencyShortName);
                builder.Append($" {Commitment.CommitmentAmount:n0}");
                Headline = builder.ToString();
                if (cashflows.Count==0)
                {
                    builder.Append("  Es sind keine Cashflows gespeichert.");
                    Headline = builder.ToString();
                }
            }
        }
    }
}
