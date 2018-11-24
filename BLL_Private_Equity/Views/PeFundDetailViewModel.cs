using BLL_DataAccess;
using BLL_DataModels;
using BLL_Private_Equity.Berechnungen;
using BLL_Private_Equity.Events;
using BLL_Infrastructure;
using BLL_Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Views
{
    public class PeFundDetailViewModel : HqtBindableBase, IRegionManagerAware
    {
        private SubscriptionToken accountListToken;
        private SubscriptionToken initiatorToken;
        // List for Comboboxes
        public List<Currency> Currencies { get; } = (List<Currency>)ComboboxLists.GetCurrencies();
        public List<FundManager> FundManagers { get; } = (List<FundManager>)ComboboxLists.GetFundManagers();
        public List<FundType> FundTypes { get; } = (List<FundType>)ComboboxLists.GetFundTypes();
        public List<FundGeography> Geographies { get; } = (List<FundGeography>)ComboboxLists.GetFundGeographies();
        public List<FundCompanySize> CompanySizes { get; } = (List<FundCompanySize>)ComboboxLists.GetFundCompanySizes();
        public List<Initiator> Initiators { get; private set; } = (List<Initiator>)ComboboxLists.GetInitiators();
        public List<PeFund> FeederFunds { get; private set; } = null;

        public InteractionRequest<INotification> NotificationRequest { get; set; }
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }
        public DelegateCommand LoadedCommand { get; set; }
        public DelegateCommand EditInitiatorCommand { get; set; }
        public DelegateCommand SaveChangesCommand { get; set; }
        public DelegateCommand UndoChangesCommand { get; set; }
        public DelegateCommand RemoveFundCommand { get; set; }
        public DelegateCommand ShowCommitmentsCommand { get; set; }
        public DelegateCommand ExportInvestorListCommand { get; set; }
        public DelegateCommand ShowCashFlowsCommand { get; set; }
        public DelegateCommand NewCashFlowCommand { get; set; }
        public DelegateCommand ShowPsPlusCashFlowsCommand { get; set; }
        public DelegateCommand EditCashFlowCommand { get; set; }
        public DelegateCommand EditPcapsCommand { get; set; }


        private bool hasHeadQuarter =false;

        public bool HasHeadQuarter
        {
            get { return this.hasHeadQuarter; }
            set { SetProperty(ref hasHeadQuarter, value); }
        }


        private bool canRemoveFund =false;

        public bool CanRemoveFund
        {
            get { return this.canRemoveFund; }
            set { SetProperty(ref canRemoveFund, value); }
        }

        private bool cashFlowInformationExists = false;

        public bool CashFlowInformationExists
        {
            get { return this.cashFlowInformationExists; }
            set { SetProperty(ref cashFlowInformationExists, value); }
        } 

        private PeFund selectedFund;

        public PeFund SelectedFund
        {
            get { return this.selectedFund; }
            set { SetProperty(ref selectedFund, value); }
        }
        
        private readonly IEventAggregator eventAggregator;
        private IRegionManager regionManager;
        public IRegionManager RegionManager { get; set; }
        public PeFundDetailViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;

            NotificationRequest = new InteractionRequest<INotification>();
            ConfirmationRequest = new InteractionRequest<IConfirmation>();
            LoadedCommand = new DelegateCommand(OnLoaded);
            EditInitiatorCommand = new DelegateCommand(OnEditInitiator);
            SaveChangesCommand = new DelegateCommand(OnSaveChanges);
            UndoChangesCommand = new DelegateCommand(OnUndoChanges);
            RemoveFundCommand = new DelegateCommand(OnRemoveFund).ObservesCanExecute(() => CanRemoveFund);
            ShowCommitmentsCommand = new DelegateCommand(OnShowCommitments);
            ExportInvestorListCommand = new DelegateCommand(OnExportInvestorList);
            NewCashFlowCommand = new DelegateCommand(OnNewCashFlow);
            ShowCashFlowsCommand = new DelegateCommand(OnShowCashFlows);
            ShowPsPlusCashFlowsCommand = new DelegateCommand(OnShowPsPlusCashFlows);
            EditCashFlowCommand = new DelegateCommand(OnEditCashFlow).ObservesCanExecute(() => CashFlowInformationExists);
            EditPcapsCommand = new DelegateCommand(OnEditPcaps);
            
            if (FeederFunds == null)
            {
               var task = LoadPefundsAsync();               
            }

        }

        private void OnEditPcaps()
        {
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Fund", SelectedFund);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.EditPcaps, parameter);
        }

        private void OnShowPsPlusCashFlows()
        {
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Fund", SelectedFund);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ShowPsPlusCashFlows, parameter);
        }

        private void OnShowCashFlows()
        {
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Fund", SelectedFund);            

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.PeFundCashFlows, parameter);
        }

        private void OnEditCashFlow()
        {
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Fund", SelectedFund);
            parameter.Add("Status", false);             // false: use existing cash flow information

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ControlCashFlow, parameter);
        }

        private void OnNewCashFlow()
        {
            if (SelectedFund.BankAccounts.Count==0)
            {
                NotificationRequest.Raise(new Notification()
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = "Für diesen Fonds sind noch keine Bankkonten erfasst. Es können keine Cash Flows eingegeben werden"
                });
                return;
            }
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Fund", SelectedFund);
            parameter.Add("Status", true);             // true: start new cash flow information

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ControlCashFlow, parameter);
        }

        private void OnExportInvestorList()
        {
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Fund", SelectedFund);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ExportInvestorList, parameter);
        }

        private void OnRemoveFund()
        {
            ConfirmationRequest.Raise(new Confirmation()
            {
                Title = ApplicationNames.NotificationTitle,
                Content = $"Möchten Sie den Fund {SelectedFund.FundHqTrustNumber} {SelectedFund.FundShortName} wirklich löschen?"
            }, (OnConfirmationResponse));
        }

        private void OnConfirmationResponse(IConfirmation obj)
        {
            if (!obj.Confirmed) return;

            // Fund wird gelöscht
            try
            {
                PefundAccess.RemovePeFund(SelectedFund);
            }
            catch (Exception ex)
            {
                NotificationRequest.Raise(new Notification()
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = ex.Message
                });
                return;
            }


            // InvestorSelection wird informiert

            PeFundCollectionAction parameter = new PeFundCollectionAction()
            {
                action = CollectionAction.removed,
                fund = SelectedFund
            };
            eventAggregator.GetEvent<PeFundCollectionActionEvent>().Publish(parameter);

            // Unsubscribe Event(s)
            eventAggregator.GetEvent<AccountCollectionEvent>().Unsubscribe(accountListToken);

            // get the current active view (myself) and remove that view from tabcontrol
            var view = regionManager.Regions[RegionNames.TabControlRegion].ActiveViews.ElementAt(0);
            regionManager.Regions[RegionNames.TabControlRegion].Remove(view);
        }

        private void OnShowCommitments()
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("Fund", SelectedFund);
            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ShowPeFundCommitments, parameters);
        }

        private void OnUndoChanges()
        {
            SelectedFund = PefundAccess.GetPeFundById(SelectedFund.Id);
        }

        private void OnSaveChanges()
        {
            UpdateOrInsertPeFund();
        }
        private void UpdateOrInsertPeFund()
        {
            try
            {
                if (SelectedFund.Id == 0)
                {
                    PefundAccess.InsertPeFund(SelectedFund);

                    //PeFundSelection will be informed about a new PeFund to add the new investor to its list
                    PeFundCollectionAction parameter = new PeFundCollectionAction()
                    {
                        action = CollectionAction.added,
                        fund = SelectedFund
                    };
                    eventAggregator.GetEvent<PeFundCollectionActionEvent>().Publish(parameter);
                    eventAggregator.GetEvent<StatusBarEvent>().Publish($"Der PeFund  {SelectedFund.FundShortName} wurde in die Datenbank eingetragen");
                }
                else
                {
                    // update investor

                    PefundAccess.UpdatePeFund(SelectedFund);

                    //InvestorSelection will be informed about changes of an Investor to show the new information
                    PeFundCollectionAction parameter = new PeFundCollectionAction()
                    {
                        action = CollectionAction.updated,
                        fund = SelectedFund
                    };
                    eventAggregator.GetEvent<PeFundCollectionActionEvent>().Publish(parameter);
                    eventAggregator.GetEvent<StatusBarEvent>().Publish($"Der Fund {SelectedFund.FundShortName} wurde in der Datenbank geändert");
                }
            }
            catch (Exception ex)
            {
                NotificationRequest.Raise(new Notification()
                { Title = ApplicationNames.NotificationTitle, Content = ex.Message });
            }
        }

        private void OnEditInitiator()
        {
            Initiator initiator = new Initiator();
            if (SelectedFund.InitiatorId != null) initiator = PefundAccess.GetInitiatorById((int)SelectedFund.InitiatorId);
            if (SelectedFund.Initiator== null || SelectedFund.InitiatorId == 0)
                initiator = new Initiator();
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("Initiator", initiator);
            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.InitiatorDetail, parameters);

            initiatorToken = eventAggregator.GetEvent<InitiatorUpdatedEvent>().Subscribe(OnInitiatorUpdated);
        }

        private void OnInitiatorUpdated(string obj)
        {
            Initiators = (List<Initiator>)ComboboxLists.GetInitiators();
            RaisePropertyChanged("Initiators");
            eventAggregator.GetEvent<InitiatorUpdatedEvent>().Unsubscribe(initiatorToken);
        }

        private void OnLoaded()
        {
            AccountCollection collection = new AccountCollection()
            {
                InvestorId = 0,
                PeFundId = SelectedFund.Id,
                CurrencyId = (int)SelectedFund.CurrencyId,
                Action = "Init",
                AccountList = new List<BankAccount>()
            };

            foreach(BankAccount account in SelectedFund.BankAccounts)
            {
                collection.AccountList.Add(account);
            }

            eventAggregator.GetEvent<AccountCollectionEvent>().Publish(collection);
            accountListToken = eventAggregator.GetEvent<AccountCollectionEvent>().Subscribe(OnAccountListChanged);
        }

        private void OnAccountListChanged(AccountCollection collection)
        {
            if (!collection.Action.Equals("Update")) return;
            if (collection.PeFundId != SelectedFund.Id) return;
            SelectedFund.BankAccounts = new ObservableCollection<BankAccount>(collection.AccountList);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
           SelectedFund = navigationContext.Parameters["Fund"] as PeFund;
            // remove fund from List of feeder funds
            if (SelectedFund.Id != 0 && FeederFunds != null) FeederFunds.Remove(SelectedFund);
            if (string.IsNullOrEmpty( SelectedFund.FundHqTrustNumber))
            {
                TabTitle = SelectedFund.FundShortName;
            }
            else
            {
                TabTitle = SelectedFund.FundHqTrustNumber;
            }
            RaisePropertyChanged("SelectedPeFund");

            // check whether cashFlowInformation exists
            DirectoryHelper.CheckDirectory($"TextFiles");
            FileInfo textFileInfo = DirectoryHelper.GetTextFileName(SelectedFund.Id);
            if (textFileInfo.Exists) CashFlowInformationExists = true;

            if (SelectedFund.CurrencyId == null) SelectedFund.CurrencyId = ComboboxLists.GetEuroCurrency().Id; 
            if (SelectedFund.Initiator != null)
            {
                if (!string.IsNullOrEmpty(SelectedFund.Initiator.InitiatorAddress.Street)) HasHeadQuarter = true;
            }           

            // Fund can be removed if the fund has no commitments

            if (!PefundAccess.FundHasCommitments(SelectedFund)) CanRemoveFund = true;
            // CanShowPsPlusCashFlows = (PefundAccess.BeteiligungsNummerExistsMoreThanOnce(SelectedFund));

        }
        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            PeFund newFund = navigationContext.Parameters["Fund"] as PeFund;
            if (newFund.Id == SelectedFund.Id) return true;
            return false;
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            UpdateOrInsertPeFund();
            eventAggregator.GetEvent<AccountCollectionEvent>().Unsubscribe(accountListToken);
        }

        private async Task LoadPefundsAsync()
        {            
            FeederFunds = new List<PeFund>(await PefundAccess.GetAllPefundsAsync());           

        }
    }
}
