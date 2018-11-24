using BLL_DataAccess;
using BLL_DataModels;
using BLL_Infrastructure;
using BLL_Prism;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;
using Telerik.Windows.Data;
using System.Collections.ObjectModel;
using Prism.Events;
using BLL_Private_Equity.Events;
using Prism.Interactivity.InteractionRequest;

namespace BLL_Private_Equity.Views
{
    public class InvestorDetailsViewModel: HqtBindableBase, IRegionManagerAware
    {
        string[] pNames = new string[]
            {"AddressName", "ConfidentialLetter", "Street", "Street2", "ZipCode", "City", "Country", "CompanyName"};

        SubscriptionToken accountListToken;
        public List<Advisor> Advisors { get; } = (List<Advisor>)ComboboxLists.GetAdvisors();
        public List<ClientAdvisor> ClientAdvisors { get; } = (List<ClientAdvisor>)ComboboxLists.GetClientAdvisors();
        public List<Currency> Currencies { get; } = (List<Currency>)ComboboxLists.GetCurrencies();
        public List<Country> Countries { get; } = (List<Country>)ComboboxLists.GetCountries();

        public IEnumerable<Telerik.Windows.Data.EnumMemberViewModel> InvestorGroups { get; } = EnumDataSource.FromType<InvestorGroup>();
        public IEnumerable<EnumMemberViewModel> Kagbs { get; } = EnumDataSource.FromType<Kagb>();
        public IEnumerable<EnumMemberViewModel> Mifids { get; } = EnumDataSource.FromType<Mifid>();

        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }
        public InteractionRequest<INotification> NotificationRequest { get; set; }
        public DelegateCommand ShowChildWindowCommand { get; set; }
        public DelegateCommand LoadedCommand { get; set; }
        public DelegateCommand AddNewEmailAccountCommand { get; set; }
        public DelegateCommand AddNewTaxInformationCommand { get; set; }
        public DelegateCommand AddNewInvestorToDoCommand { get; set; }
        public DelegateCommand SaveChangesCommand { get; set; }
        public DelegateCommand UndoChangesCommand { get; set; }
        public DelegateCommand ShowCommitmentsCommand { get; set; }
        public DelegateCommand DeleteInvestorCommand { get; set; }
 

        private ObservableCollection<EMailAccount> eMailAccounts;

        public ObservableCollection<EMailAccount> EMailAccounts
        {
            get { return this.eMailAccounts; }
            set { SetProperty(ref eMailAccounts, value); }
        }


        private ObservableCollection<TaxInformation> taxInformation;

        public ObservableCollection<TaxInformation> TaxInformation
        {
            get { return this.taxInformation; }
            set { SetProperty(ref taxInformation, value); }
        }


        private ObservableCollection<InvestorToDo> investorToDo;

        public ObservableCollection<InvestorToDo> InvestorToDo
        {
            get { return this.investorToDo; }
            set { SetProperty(ref investorToDo, value); }
        }

        private WindowState childWindowAccountState = WindowState.Closed;

        public WindowState ChildWindowAccountState
        {
            get { return this.childWindowAccountState; }
            set { SetProperty(ref childWindowAccountState, value); }
        }

        private string previewAddress;

        public string PreviewAddress
        {
            get { return this.previewAddress; }
            set { SetProperty(ref previewAddress, value); }
        }

        private bool canDeleteInvestor = false;

        public bool CanDeleteInvestor
        {
            get { return this.canDeleteInvestor; }
            set { SetProperty(ref canDeleteInvestor, value); }
        }
        private Investor investor;

        public Investor Investor
        {
            get { return this.investor; }
            set { SetProperty(ref investor, value); }
        }

        public bool CreateRegionManagerScope => true;

        public IRegionManager RegionManager { get; set; }

        InvestorAccess investorAccess = new InvestorAccess();
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        

        public InvestorDetailsViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            ShowChildWindowCommand = new DelegateCommand(OnShowChildWindow);
            LoadedCommand = new DelegateCommand(OnLoaded);
            AddNewEmailAccountCommand = new DelegateCommand(OnAddNewEmailAccount);
            AddNewTaxInformationCommand = new DelegateCommand(OnAddNewTaxInformation);
            AddNewInvestorToDoCommand = new DelegateCommand(OnAddNewInvestorToDo);
            SaveChangesCommand = new DelegateCommand(OnSaveChanges);
            UndoChangesCommand = new DelegateCommand(OnUndoChanges);
            ShowCommitmentsCommand = new DelegateCommand(OnShowCommitments);
            DeleteInvestorCommand = new DelegateCommand(OnDeleteInvestor).ObservesCanExecute(() => CanDeleteInvestor);

            ConfirmationRequest = new InteractionRequest<IConfirmation>();
            NotificationRequest = new InteractionRequest<INotification>();
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;

            
        }

        private void OnDeleteInvestor()
        {
            ConfirmationRequest.Raise(new Confirmation()
            {
                Title = ApplicationNames.NotificationTitle,
                Content = $"Möchten Sie den Investor {Investor.InvestorHqTrustAccount} {Investor.IName.FullName} wirklich löschen?"
            }, (OnConfirmationResponse));
        }

        private void OnConfirmationResponse(IConfirmation obj)
        {
            if (!obj.Confirmed) return;

            // Investor wird gelöscht
            try
            {
                investorAccess.RemoveInvestor(Investor);
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

            InvestorCollectionAction parameter = new InvestorCollectionAction()
            {
                action = CollectionAction.removed,
                investor = Investor
            };
            eventAggregator.GetEvent<InvestorCollectionActionEvent>().Publish(parameter);

            // Unsubscribe Event(s)
            eventAggregator.GetEvent<AccountCollectionEvent>().Unsubscribe(accountListToken);

            // get the current active view (myself) and remove that view from tabcontrol
            var view = regionManager.Regions[RegionNames.TabControlRegion].ActiveViews.ElementAt(0);
            regionManager.Regions[RegionNames.TabControlRegion].Remove(view);
                        
        }

        private void OnShowCommitments()
        {
            // request navigate to InvestorCommitmentView
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Investor", Investor);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ShowInvestorCommitments, parameter);
        }

        private void OnUndoChanges()
        {
           
        }

        private void OnSaveChanges()
        {
            UpdateOrInsertInvestor();
        }

        private void UpdateOrInsertInvestor()
        {
            try
            {
                if (Investor.Id == 0)
                {
                    investorAccess.InsertInvestor(Investor);

                    //InvestorSelection will be informed about a new Investor to add the new investor to its list
                    InvestorCollectionAction parameter = new InvestorCollectionAction()
                    {
                        action = CollectionAction.added,
                        investor = Investor
                    };
                    eventAggregator.GetEvent<InvestorCollectionActionEvent>().Publish(parameter);
                    eventAggregator.GetEvent<StatusBarEvent>().Publish($"Der Investor  {Investor.InvestorReference} wurde in die Datenbank eingetragen");
                }
                else
                {
                    // update investor

                    investorAccess.UpdateInvestor(Investor);

                    //InvestorSelection will be informed about changes of an Investor to show the new information
                    InvestorCollectionAction parameter = new InvestorCollectionAction()
                    {
                        action = CollectionAction.updated,
                        investor = Investor
                    };
                    eventAggregator.GetEvent<InvestorCollectionActionEvent>().Publish(parameter);
                    eventAggregator.GetEvent<StatusBarEvent>().Publish($"Der Investor {Investor.IName.FullName} wurde in der Datenbank geändert");
                }
            }
            catch (Exception ex)
            {
                NotificationRequest.Raise(new Notification()
                { Title = ApplicationNames.NotificationTitle, Content = ex.Message });
            }
        }

        private void OnAddNewInvestorToDo()
        {
            InvestorToDo toDo = new InvestorToDo()
            {
                InvestorId = Investor.Id
            };
            InvestorToDo.Add(toDo);
            RaisePropertyChanged("InvestorToDo");
        }

        private void OnAddNewTaxInformation()
        {
            TaxInformation info = new TaxInformation();
            info.InvestorId = Investor.Id;
            TaxInformation.Add(info);
        }

        private void OnAddNewEmailAccount()
        {
            EMailAccount newAccount = new EMailAccount();
            newAccount.EmailAddress = "bitte gültige Adresse eingeben";
            newAccount.InvestorId = Investor.Id;
            EMailAccounts.Add(newAccount);
        }

      

        private void Investor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string p = pNames.FirstOrDefault(n => n.Equals(e.PropertyName));
            if (p == null) return;

            CreatePreviewAddress();
        }

        private void CreatePreviewAddress()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (Investor.ConfidentialLetter)
                stringBuilder.Append("Persönlich/Vertraulich" + Environment.NewLine);
            else
                stringBuilder.Append(" " + Environment.NewLine);
            stringBuilder.Append(Investor.CompanyName + Environment.NewLine);
            stringBuilder.Append(Investor.IName.AddressName + Environment.NewLine);
            stringBuilder.Append(Investor.PrivateAddress.Street + Environment.NewLine);
            stringBuilder.Append(Investor.PrivateAddress.Street2 + Environment.NewLine);
            stringBuilder.Append(Investor.PrivateAddress.ZipCode + " " + Investor.PrivateAddress.City);
            if (!string.IsNullOrEmpty(Investor.PrivateAddress.Country))
                stringBuilder.Append(Environment.NewLine + Investor.PrivateAddress.Country);
            PreviewAddress = stringBuilder.ToString();
        }

        private void OnLoaded()
        {
            Investor.PropertyChanged -= Investor_PropertyChanged;
            Investor.PrivateAddress.PropertyChanged -= Investor_PropertyChanged;
            Investor.IName.PropertyChanged -= Investor_PropertyChanged;
            Investor.PropertyChanged += Investor_PropertyChanged;
            Investor.PrivateAddress.PropertyChanged += Investor_PropertyChanged;
            Investor.IName.PropertyChanged += Investor_PropertyChanged;

            // inform EditAccountCollections about Accounts
            AccountCollection collection = new AccountCollection()
            {
                InvestorId = investor.Id,
                PeFundId = 0,
                CurrencyId = (int)investor.CurrencyId,
                Action = "Init",
                AccountList = Investor.BankAccounts
            };

            eventAggregator.GetEvent<AccountCollectionEvent>().Publish(collection);
            accountListToken = eventAggregator.GetEvent<AccountCollectionEvent>().Subscribe(OnAccountListChanged);
        }

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            // automatic update:
            UpdateOrInsertInvestor();
            eventAggregator.GetEvent<AccountCollectionEvent>().Unsubscribe(accountListToken);
        }

        private void OnAccountListChanged(AccountCollection collection)
        {
            if (!collection.Action.Equals("Update")) return;
            if (collection.InvestorId != Investor.Id) return;
            Investor.BankAccounts = collection.AccountList;
        }

        private void OnShowChildWindow()
        {
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Accounts", Investor.BankAccounts);
            regionManager.RequestNavigate(RegionNames.AccountRegion, ViewNames.AccountEditWindow, parameter);
            
            ChildWindowAccountState = WindowState.Open;

        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Investor  = navigationContext.Parameters["Investor"] as Investor;
            if (Investor == null)
            {
                TabTitle = "unbekannt";
                Investor = new Investor();
                EMailAccounts = new ObservableCollection<EMailAccount>(Investor.EMailAccounts);
                TaxInformation = new ObservableCollection<TaxInformation>(Investor.TaxInformations);
                InvestorToDo = new ObservableCollection<InvestorToDo>();
                Investor.CurrencyId = ComboboxLists.GetEuroCurrency().Id;
                Investor.Currency = ComboboxLists.GetEuroCurrency();                
            }
            else
            {
                TabTitle = Investor.InvestorHqTrustAccount;
                if (string.IsNullOrEmpty( TabTitle))
                { TabTitle = Investor.IName.LastName; }

                Investor = investorAccess.GetInvestorDetailsById(Investor.Id);
                EMailAccounts = new ObservableCollection<EMailAccount>(Investor.EMailAccounts);
                TaxInformation = new ObservableCollection<TaxInformation>(Investor.TaxInformations);
                InvestorToDo = new ObservableCollection<InvestorToDo>(investorAccess.GetToDosForInvestor(Investor.Id));
                if (Investor.Commitments.Count == 0) CanDeleteInvestor = true;

                CreatePreviewAddress();
            }
            accountListToken = eventAggregator.GetEvent<AccountCollectionEvent>().Subscribe(OnAccountListChanged);

        }
        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            Investor newInvestor = navigationContext.Parameters["Investor"] as Investor;
            //newInvestor is null if the user wants to add a new Investor: a new instance is neccessary
            if (newInvestor == null) return false;
            if (newInvestor.Id == Investor.Id) return true;
            return false;
        }
    }
}
