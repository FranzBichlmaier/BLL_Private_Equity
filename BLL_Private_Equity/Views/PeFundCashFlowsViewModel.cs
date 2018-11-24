using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using BLL_DataAccess;
using BLL_DataModels;
using BLL_Infrastructure;
using BLL_Prism;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;

namespace BLL_Private_Equity.Views
{
    public class PeFundCashFlowsViewModel: BLL_Prism.HqtBindableBase, IRegionManagerAware
    {
        ObservableCollection<UniqueCashFlowNumber> uniqueNumbers;
        private InvestorAccess investorAccess = new InvestorAccess();
        


        private List<InvestorCashFlow> investorCashFlows;

        public List<InvestorCashFlow> InvestorCashFlows
        {
            get { return this.investorCashFlows; }
            set { SetProperty(ref investorCashFlows, value); }
        }

        private ICollectionView uniqueCashFlows;

        public ICollectionView UniqueCashFlows
        {
            get { return this.uniqueCashFlows; }
            set { SetProperty(ref uniqueCashFlows, value); }
        }
        private PeFund fund;

        public PeFund Fund
        {
            get { return this.fund; }
            set { SetProperty(ref fund, value); }
        }

        private bool cashFlowsLoading;

        public bool CashFlowsLoading
        {
            get { return this.cashFlowsLoading; }
            set { SetProperty(ref cashFlowsLoading, value); }
        }


        private bool canUserDeleteCashFlows;

        public bool CanUserDeleteCashFlows
        {
            get { return this.canUserDeleteCashFlows; }
            set { SetProperty(ref canUserDeleteCashFlows, value); }
        }

        public ICommand DeleteCashFlowsCommand { get; set; }
        public InteractionRequest<INotification> NotificationRequest { get; set; }
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }
        public IRegionManager RegionManager { get ; set ; }

        public PeFundCashFlowsViewModel()
        {
            DeleteCashFlowsCommand = new DelegateCommand(OnDeleteCashFlows).ObservesCanExecute(() => CanUserDeleteCashFlows);
            NotificationRequest = new InteractionRequest<INotification>();
            ConfirmationRequest = new InteractionRequest<IConfirmation>();
        }

        private void OnDeleteCashFlows()
        {
            ConfirmationRequest.Raise(new Confirmation()
            {
                Title = ApplicationNames.NotificationTitle,
                Content = "Sollen die ausgewählten Cashflows wirklich gelöscht werden?"
            }, OnDeleteCashFlowConfirmation);
           
        }

        private void OnDeleteCashFlowConfirmation(IConfirmation obj)
        {
           if (obj.Confirmed)
            {
                try
                {
                    int number = investorAccess.DeleteInvestorCashFlows(InvestorCashFlows);
                    NotificationRequest.Raise(new Notification()
                    {
                        Title = ApplicationNames.NotificationTitle,
                        Content = $"Es wurden {number.ToString()} Cashflows gelöscht!"
                    });
                    // remove CurrentItem from List
                    UniqueCashFlowNumber item = UniqueCashFlows.CurrentItem as UniqueCashFlowNumber;
                    uniqueNumbers.Remove(item);
                }
                catch (Exception ex)
                {
                    NotificationRequest.Raise(new Notification()
                    {
                        Title = ApplicationNames.NotificationTitle,
                        Content = $"Beim Löschen von Cashflows trat ein Fehler auf: {ex.Message}"
                    });
                }
            }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            PeFund newFund = navigationContext.Parameters["Fund"] as PeFund;
            if (newFund.Id == Fund.Id) return true;
            return false;
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Fund = navigationContext.Parameters["Fund"] as PeFund;
            TabTitle = Fund.FundShortName + " CashFlows";
            if (UniqueCashFlows != null) UniqueCashFlows.CurrentChanged -= UniqueCashFlows_CurrentChanged;

            uniqueNumbers = new ObservableCollection<UniqueCashFlowNumber>(PefundAccess.GetCashFlowsForFund(Fund.Id));           

            UniqueCashFlows = CollectionViewSource.GetDefaultView(uniqueNumbers);
            UniqueCashFlows.CurrentChanged += UniqueCashFlows_CurrentChanged;
            RaisePropertyChanged("UniqueCashFlows");
            var task = AddInvestorCashFlowsAsync();
        }

        private void UniqueCashFlows_CurrentChanged(object sender, EventArgs e)
        {
            if (UniqueCashFlows.CurrentItem == null)
            {
                InvestorCashFlows = new List<InvestorCashFlow>();
                return;
            }
            UniqueCashFlowNumber item = UniqueCashFlows.CurrentItem as UniqueCashFlowNumber;
            InvestorCashFlows = new List<InvestorCashFlow>(item.InvestorCashFlows);
            RaisePropertyChanged("InvestorCashFlows");
            CanUserDeleteCashFlows = false;
            foreach(InvestorCashFlow cf in InvestorCashFlows)
            {
                if (cf.PsPlusId != 0)
                {
                    CanUserDeleteCashFlows = false;
                    break;
                }
                CanUserDeleteCashFlows = true;
            }
        }

        private async Task AddInvestorCashFlowsAsync()
        {
            CashFlowsLoading = true;
            foreach(UniqueCashFlowNumber item in uniqueNumbers)
            {
                item.InvestorCashFlows = await investorAccess.GetInvestorCashFlowsByUniqueNumberAsync(item.Id);
                foreach(InvestorCashFlow cf in item.InvestorCashFlows)
                {
                    item.CashFlowAmount += cf.CashFlowAmount;
                }
                if (item.InvestorCashFlows.Count > 0)
                {
                    item.CashFlowType = item.InvestorCashFlows.ElementAt(0).CashFlowType;
                }        
            }
            // remove all UniqueCashFlows where number of InvestorCashFlows == 0
            {
                for(int i = uniqueNumbers.Count-1;i>=0;i--)
                {
                    UniqueCashFlowNumber ucn = uniqueNumbers.ElementAt(i);
                    if (ucn.InvestorCashFlows.Count == 0) uniqueNumbers.RemoveAt(i);
                }
            }
            CashFlowsLoading = false;
        }
        
    }
}
