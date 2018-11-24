using BLL_DataAccess;
using BLL_DataModels;
using BLL_Private_Equity.Events;
using BLL_Prism;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BLL_Private_Equity.Views
{
    public class EditAccountCollectionsViewModel: HqtBindableBase    
    {
        private readonly IEventAggregator eventAggregator;
        private int investorId = 0;
        private int peFundId = 0;
        private int currencyId = 0;
        private CurrencyAccess currencyAccess = new CurrencyAccess();
        private Currency defaultCurrency;
        public List<Currency> Currencies { get; } = (List<Currency>)ComboboxLists.GetCurrencies();
        public List<ClientAdvisor> ClientAdvisors { get; } = (List<ClientAdvisor>)ComboboxLists.GetClientAdvisors();


        private bool canAddNewAccount =true;

        public bool CanAddNewAccount
        {
            get { return this.canAddNewAccount; }
            set { SetProperty(ref canAddNewAccount, value); }
        }

        public DelegateCommand AddNewAccountCommand { get; set; }
        public DelegateCommand AccountDeletedCommand { get; set; }


        private BankAccount selectedBankAccount = null;

        public BankAccount SelectedBankAccount
        {
            get { return this.selectedBankAccount; }
            set { SetProperty(ref selectedBankAccount, value); }
        }


        private ObservableCollection<BankAccount> myBankAccounts;

       public ICollectionView BankAccounts { get; set; }

        public EditAccountCollectionsViewModel(IEventAggregator eventAggregator)
        {
            AddNewAccountCommand = new DelegateCommand(OnAddNewAccount).ObservesCanExecute(() => CanAddNewAccount);
            AccountDeletedCommand = new DelegateCommand(OnAccountDeleted);
            this.eventAggregator = eventAggregator;
            eventAggregator.GetEvent<AccountCollectionEvent>().Subscribe(OnAccountCollectionEvent);
        }

        // inform caller about new List
        private void OnAccountDeleted()
        {
            AccountCollection collection = new AccountCollection()
            {
                InvestorId = investorId,
                PeFundId = peFundId,
                Action = "Update",
                AccountList = myBankAccounts.ToList()
            };
            eventAggregator.GetEvent<AccountCollectionEvent>().Publish(collection);

        }

        private void OnAddNewAccount()
        {
            BankAccount newAccount = new BankAccount()
            {
                InvestorId = investorId,
                PefundId = peFundId,
                CurrencyId=currencyId
                
            };
            myBankAccounts.Add(newAccount);
            BankAccounts.MoveCurrentToLast();   // sets new Account as selected Account
            RaisePropertyChanged("BankAccounts");
            CanAddNewAccount = false;
        }

        private void OnAccountCollectionEvent(AccountCollection collection)
        {
            if (investorId != 0) return;
            if (peFundId != 0) return;
            if(collection.Action == "Init")
            {
                investorId = collection.InvestorId;
                peFundId = collection.PeFundId;
                currencyId = collection.CurrencyId;
                defaultCurrency = new Currency();
                defaultCurrency = currencyAccess.GetCurrencyById(collection.CurrencyId);
                myBankAccounts = new ObservableCollection<BankAccount>(collection.AccountList);
                BankAccounts = CollectionViewSource.GetDefaultView(myBankAccounts);                
                BankAccounts.CurrentChanged += BankAccounts_CurrentChanged;
                if (BankAccounts.IsEmpty)
                {
                    OnAddNewAccount();                    
                }
                else
                {
                    // select first account
                    foreach (BankAccount account in BankAccounts)
                    {
                        if (account.Currency == null)
                        {
                            account.CurrencyId = defaultCurrency.Id;
                            account.Currency = defaultCurrency;
                            account.CurrencyName = defaultCurrency.CurrencyShortName;
                        }
                        else
                        {
                            account.CurrencyName = account.Currency.CurrencyShortName;
                        }                        
                    }
                    BankAccounts.MoveCurrentToFirst();
                    SelectedBankAccount = BankAccounts.CurrentItem as BankAccount;
                    SelectedBankAccount.PropertyChanged += SelectedBankAccount_PropertyChanged;
                    RaisePropertyChanged("BankAccounts");
                }
               
            }
        }

        private void BankAccounts_CurrentChanged(object sender, EventArgs e)
        {
            if (SelectedBankAccount != null) SelectedBankAccount.PropertyChanged -= SelectedBankAccount_PropertyChanged;
            SelectedBankAccount = BankAccounts.CurrentItem as BankAccount;
            if (SelectedBankAccount == null)
            {
                AccountCollection collection = new AccountCollection()
                {
                    InvestorId = investorId,
                    PeFundId = peFundId,
                    Action = "Update",
                    AccountList = new List<BankAccount>()
                };
                eventAggregator.GetEvent<AccountCollectionEvent>().Publish(collection);
                return;
            }
            SelectedBankAccount.PropertyChanged += SelectedBankAccount_PropertyChanged;
        }

        private void SelectedBankAccount_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrencyId")
            {
                Currency selectedCurrency = currencyAccess.GetCurrencyById(SelectedBankAccount.CurrencyId);
                SelectedBankAccount.CurrencyName = selectedCurrency.CurrencyShortName;
                RaisePropertyChanged("BankAccounts");
            }
            if (e.PropertyName == "Iban" || e.PropertyName =="AccountNumber")
            {
                CanAddNewAccount = true;
            }

            // Send changes back to caller
            AccountCollection collection = new AccountCollection()
            {
                InvestorId = investorId,
                PeFundId = peFundId,
                Action = "Update",
                AccountList = myBankAccounts.ToList()
            };
            eventAggregator.GetEvent<AccountCollectionEvent>().Publish(collection);
        }
    }
}
