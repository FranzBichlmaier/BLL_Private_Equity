using BLL_DataAccess;
using BLL_DataModels;
using BLL_Private_Equity.Berechnungen;
using BLL_Infrastructure;
using BLL_Prism;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Telerik.Windows.Data;

namespace BLL_Private_Equity.Views
{
    public class ShowPsPlusCashFlowsViewModel: HqtBindableBase
    {
        private PeFund fund = new PeFund();
        private string fundNames = string.Empty;

        public ICommand SearchForTransactionNumberCommand { get; set; }
        public InteractionRequest<INotification> NotificationRequest { get; set; }

        public string FundNames
        {
            get { return this.fundNames; }
            set { SetProperty(ref fundNames, value); }
        }


        private long psPlusTransactionNumber;

        public long PsPlusTransactionNumber
        {
            get { return this.psPlusTransactionNumber; }
            set { SetProperty(ref psPlusTransactionNumber, value); }
        }

        private bool cashFlowsLoading = false;

        public bool CashFlowsLoading
        {
            get { return this.cashFlowsLoading; }
            set { SetProperty(ref cashFlowsLoading, value); }
        }
        private ObservableCollection<PsPlusCashFlow> cashFlows = null;
        public ICollectionView PsPlusCashFlows { get; set; } = null;

        public ShowPsPlusCashFlowsViewModel()
        {
            SearchForTransactionNumberCommand = new DelegateCommand(OnSearchForTransactionNuumber);
            NotificationRequest = new InteractionRequest<INotification>();
        }

        private void OnSearchForTransactionNuumber()
        {
            PsPlusCashFlow cashFlow = cashFlows.FirstOrDefault(c => c.InvestorCashFlow.PsPlusId == PsPlusTransactionNumber);
            if (cashFlow!= null)
            {
                PsPlusCashFlows.MoveCurrentTo(cashFlow);
                return;
            }
            NotificationRequest.Raise(new Notification()
            {
                Title = ApplicationNames.NotificationTitle,
                Content = $"Ein Cashflow mit der Transaktionsnummer {PsPlusTransactionNumber} wurde nicht gefunden."
            });
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            fund = navigationContext.Parameters["Fund"] as PeFund;
            TabTitle = $"PsPlus CashFlows ({fund.FundHqTrustNumber})";
            if (PsPlusCashFlows != null) PsPlusCashFlows.CurrentChanged -= PsPlusCashFlows_CurrentChanged;

            // fill collection of cashflows:
            await Task.Run(() => AddInvestorCashFlowsAsync());

            PsPlusCashFlows = CollectionViewSource.GetDefaultView(cashFlows);
            PsPlusCashFlows.CurrentChanged += PsPlusCashFlows_CurrentChanged;
            RaisePropertyChanged("PsPlusCashFlows");

            //
            // add sort criteria to CollectionViewSource
            // sort by HQTrustaccount and then by Date
            //

            PsPlusCashFlows.SortDescriptions.Clear();

            SortDescription sortDescription = new SortDescription()
            {
                Direction = ListSortDirection.Ascending,
                PropertyName = "InvestorHqTrustAccount"
            };
            PsPlusCashFlows.SortDescriptions.Add(sortDescription);

            sortDescription = new SortDescription()
            {
                Direction = ListSortDirection.Ascending,
                PropertyName = "InvestorCashFlow.EffectiveDate"                
            };
            PsPlusCashFlows.SortDescriptions.Add(sortDescription);
     

            //
            // groupdescription to CollectoinViewSource     
            // the information is grouped by HqTrustAccount
            // grouping is defined in the xaml
            //
  
        }

        private async Task AddInvestorCashFlowsAsync()
        {
            CashFlowsLoading = true;
            cashFlows = new ObservableCollection<PsPlusCashFlow>();
            List<PeFund> funds = PefundAccess.GetFundListForBeteiligungsnumer(fund.FundHqTrustNumber);
            StringBuilder builder = new StringBuilder(fund.FundHqTrustNumber + ": ");
            bool firstItem = true;
            foreach(PeFund f in funds)
            {

                //
                // add FundName to FundNames
                // 

                if (string.IsNullOrEmpty(f.FundLegalName)) f.FundLegalName = f.FundName;
                if (firstItem) builder.Append(f.FundName);
                else { builder.Append(", " + f.FundName); }
                firstItem = false;

                //
                // read commitments for each fund
                //
                var commitments = await PefundAccess.GetCommitmentsForPeFundIncludingCashFlowsAsync(f.Id);

                //
                // read cashflows for each commitment and add properties to be displayed in the gridview
                //

                foreach(InvestorCommitment commitment in commitments)
                {
                    // nur HQT Kunden verarbeiten
                    if (commitment.Investor.IsHqtClient == false) continue;
                    foreach(InvestorCashFlow cf in commitment.InvestorCashFlows)
                    {
                        PsPlusCashFlow psPlusCf = new PsPlusCashFlow()
                        {
                            InvestorCashFlow = cf,
                            InvestorHqTrustAccount = commitment.Investor.InvestorHqTrustAccount,
                            InvestorId = (int)commitment.InvestorId,
                            InvestorReference = commitment.Investor.InvestorReference
                        };
                        cashFlows.Add(psPlusCf);
                    }
                }
            }
            FundNames = builder.ToString();
            CashFlowsLoading = false;
        }

        private void PsPlusCashFlows_CurrentChanged(object sender, EventArgs e)
        {
           
        }
    }
}
