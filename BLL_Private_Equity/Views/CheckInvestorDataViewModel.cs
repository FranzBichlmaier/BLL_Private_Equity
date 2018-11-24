using BLL_DataAccess;
using BLL_DataModels;
using BLL_Private_Equity.Berechnungen;
using BLL_Private_Equity.Events;
using BLL_Infrastructure;
using BLL_Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BLL_Private_Equity.Views
{
    public class CheckInvestorDataViewModel: HqtBindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private InvestorAccess investorAccess = new InvestorAccess();

        private CashFlowInformation cashFlowInformation;

        public CashFlowInformation CashFlowInformation
        {
            get { return this.cashFlowInformation; }
            set { SetProperty(ref cashFlowInformation, value); }
        }

        private ObservableCollection<CashFlowErrors> observableCollection;

        public ObservableCollection<CashFlowErrors> Errors
        {
            get { return this.observableCollection; }
            set { SetProperty(ref observableCollection, value); }
        }

        private bool canGotoNextStep = false;

        public bool CanGotoNextStep
        {
            get { return this.canGotoNextStep; }
            set { SetProperty(ref canGotoNextStep, value); }
        }


        private CashFlowErrors selectedError;

        public CashFlowErrors SelectedError
        {
            get { return this.selectedError; }
            set { SetProperty(ref selectedError, value); }
        }
        private int errorCounter;

        public int ErrorCounter
        {
            get { return this.errorCounter; }
            set { SetProperty(ref errorCounter, value); }
        }

        public ICommand ErrorSelectedCommand { get; set; } 
        public ICommand ReloadInformationCommand { get; set; }
        public ICommand GotoNextStepCommand { get; set; }

        public CheckInvestorDataViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            ErrorSelectedCommand = new DelegateCommand(OnErrorSelected);
            ReloadInformationCommand = new DelegateCommand(OnReloadInformation);
            GotoNextStepCommand = new DelegateCommand(OnGotoNextStep).ObservesCanExecute(() => CanGotoNextStep);
        }

        private void OnGotoNextStep()
        {
            PrepareCashFlow cf = new PrepareCashFlow();
            cf.cfInfo = cashFlowInformation;
            eventAggregator.GetEvent<PrepareCashFlowEvent>().Publish(cf);
        }

        private void OnReloadInformation()
        {
             ReloadCashFlowInformation();
            Errors = new ObservableCollection<CashFlowErrors>();
            ErrorCounter = 0;
            CheckStandingData();
        }

        private void ReloadCashFlowInformation()
        {
            CashFlowInformation.Fund = PefundAccess.GetPeFundById(CashFlowInformation.Fund.Id);
            CashFlowInformation.LastUpdated = DateTime.Now.Date;
            List<InvestorCommitment> ListOfCommitments = PefundAccess.GetCommitmentsForPeFund(CashFlowInformation.Fund.Id);
            CashFlowDetail totalDetail = CashFlowInformation.DetailSummary;
            totalDetail.CommitmentAmount = 0;                                   // will be calculated while reading the investorcommitments
            CashFlowInformation.InvestorDetails = new List<CashFlowDetail>();

            if (CashFlowInformation.Fund.BankAccounts.Count == 0) totalDetail.BankAccount = null;
            else totalDetail.BankAccount = CashFlowInformation.Fund.BankAccounts.ElementAt(0);

            foreach (InvestorCommitment commitment in ListOfCommitments)
            {
                CashFlowDetail investorDetail = new CashFlowDetail()
                {
                    Investor = commitment.Investor,
                    Reference = commitment.Investor.InvestorReference,
                    BankAccount = commitment.BankAccount,
                    CommitmentAmount = commitment.CommitmentAmount,
                    InvestorCommitmentId = commitment.Id
                };
                if (!string.IsNullOrEmpty(commitment.PeFundReference)) investorDetail.Reference = commitment.PeFundReference;
                totalDetail.CommitmentAmount += commitment.CommitmentAmount;
                CashFlowInformation.InvestorDetails.Add(investorDetail);
            }
            CashFlowInformation.DetailSummary = totalDetail;
        }

        private void OnErrorSelected()
        {
            if (SelectedError == null) return;

            // if PeFundId <>0 start PeFundDetail
            // if InvestorId <>0 start InvestorDetail

            if (SelectedError.InvestorId !=0)
            {
                Investor investor = investorAccess.GetInvestorById(SelectedError.InvestorId);
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Investor", investor);
                regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.InvestorDetails, parameters);                
            }
            if (SelectedError.PeFundId != 0)
            {
                PeFund fund = PefundAccess.GetPeFundById(SelectedError.PeFundId);
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Fund", fund);
                regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.PeFundDetail, parameters);

            }
        }
        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            CashFlowInformation = navigationContext.Parameters["Info"] as CashFlowInformation;
            ErrorCounter = 0;
            Errors = new ObservableCollection<CashFlowErrors>();
            CheckStandingData();
            RaisePropertyChanged("Errors");
        }

        private void CheckStandingData()
        {
           if (cashFlowInformation.Fund.IsHqTrustFund)
            {
                if (cashFlowInformation.Fund.InitiatorId==0)
                {
                    ErrorCounter++;
                    AddError(cashFlowInformation.Fund, "Es ist kein Initiator erfasst!");
                }
                else
                {
                    if (string.IsNullOrEmpty( cashFlowInformation.Fund.Initiator.FooterLine1))
                    {
                        ErrorCounter++;
                        AddError(cashFlowInformation.Fund, "Für den Initiator sind keine Fusszeilen erfasst!");
                    }
                }
            }
           foreach(CashFlowDetail detail in cashFlowInformation.InvestorDetails)
            {
                if (string.IsNullOrEmpty(detail.Reference))
                {
                    errorCounter++;
                    AddError(detail.Investor, "Weder beim Investor noch beim Commitment wurde eine Referenz angegeben.");
                }
                if (detail.BankAccount == null)
                {
                    errorCounter++;
                    AddError(detail.Investor, "Eine Bankverbindung fehlt");
                }
                if (detail.Investor.SendEmail==true)
                {
                    if (detail.Investor.EMailAccounts.Count==0 && string.IsNullOrEmpty(detail.Investor.IName.EmailAddress))
                    {
                        errorCounter++;
                        AddError(detail.Investor, "Für diesen Investor sollen E-Mails verschickt werden. Es sind keine E-Mail-Adressen angegeben");
                    }
                }
            }
           if (Errors.Count==0)
            {
                CashFlowErrors error = new CashFlowErrors()
                {
                    InvestorId = 0,
                    PeFundId=0,
                    ErrorText = "Es wurden keine Fehler festgestellt",
                    ObjectName = string.Empty
                };
                Errors.Add(error);
                CanGotoNextStep = true;
                CashFlowInformation.InvestorsChecked = true;
            }
            else
            {
                CashFlowErrors error = new CashFlowErrors()
                {
                    InvestorId = 0,
                    PeFundId = 0,
                    ObjectName = "Zusammenfassung:",
                    ErrorText = $"Es wurden {ErrorCounter.ToString()} Fehler entdeckt."
                };
                Errors.Insert(0, error);
                if (Properties.Settings.Default.Anwender=="BLL")
                {
                    CanGotoNextStep = true;
                    CashFlowInformation.InvestorsChecked = true;
                }
            }
        }

        private void AddError(Investor investor, string v)
        {
            CashFlowErrors error = new CashFlowErrors()
            {
                InvestorId = investor.Id,
                ErrorText = v,
                ObjectName = investor.IName.FullName
            };
            Errors.Add(error);
        }

        private void AddError(PeFund fund, string v)
        {
            CashFlowErrors error = new CashFlowErrors()
            {
                PeFundId = fund.Id,
                ErrorText = v,
                ObjectName = fund.FundShortName
            };
            Errors.Add(error);
        }
    }
}
