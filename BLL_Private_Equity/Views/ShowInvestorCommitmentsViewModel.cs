using BLL_DataAccess;
using BLL_DataModels;
using BLL_Private_Equity.Berechnungen;
using BLL_Private_Equity.Comparer;
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
using System.Threading.Tasks;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace BLL_Private_Equity.Views
{
    public class ShowInvestorCommitmentsViewModel: HqtBindableBase, IRegionManagerAware
    {
        private SubscriptionToken token;
        private ObservableCollection<PeFund> availableFunds;
        public InteractionRequest<INotification> NotificationRequest { get; set; }
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }
        public ICommand GridMouseDoubleClickCommand { get; set; }
        public ICommand NewCommitmentCommand { get; set; }
        public ICommand EditCommitmentCommand { get; set; }
        public ICommand DeleteCommitmentCommand { get; set; }
        public ICommand GridViewSelectionChangedCommand { get; set; }
        public ICommand AnalyseSelectedCommitmentCommand { get; set; }
        public ICommand AnalyseAllCommitmentsCommand { get; set; }
        public ICommand AnalyseSelectionCommand { get; set; }
        public ICommand ShowCashFlowsCommand { get; set; }
        public ICommand ShowPcapsCommand { get; set; }
        public ICommand ShowPeFundCommand { get; set; }
        public ICommand CreateCommitmentReportCommand { get; set; }
        public ICommand EditInvestorCommitmentClosed { get; set; }

        public IRegionManager RegionManager { get; set; }

        private IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private InvestorAccess investorAccess = new InvestorAccess(); 
        
        
        private Investor investor;

        public Investor Investor
        {
            get { return this.investor; }
            set { SetProperty(ref investor, value); }
        }

        private InvestorCommitment selectedCommitment;

        public InvestorCommitment SelectedCommitment
        {
            get { return this.selectedCommitment; }
            set
            {
                SetProperty(ref selectedCommitment, value);
                if (SelectedCommitment == null)
                {
                    CanEditCommitment = false;
                    CanDeleteCommitment = false;
                }
                else
                {
                    CanDeleteCommitment = (SelectedCommitment.InvestorCashFlows.Count > 0) ? false : true;
                    CanEditCommitment = true;
                }               
            }
        }

        private List<InvestorCommitment>  selectedCommitments;

        public  List<InvestorCommitment> SelectedCommitments
        {
            get { return this.selectedCommitments; }
            set { SetProperty(ref selectedCommitments, value); }
        }

        private WindowState editCommitmentWindowState = WindowState.Closed;

        public WindowState EditCommitmentWindowState
        {
            get { return this.editCommitmentWindowState; }
            set { SetProperty(ref editCommitmentWindowState, value); }
        }
        private bool canEditCommitment = false;

        public bool CanEditCommitment
        {
            get { return this.canEditCommitment; }
            set { SetProperty(ref canEditCommitment, value); }
        }

        private bool canDeleteCommitment = false;

        public bool CanDeleteCommitment
        {
            get { return this.canDeleteCommitment; }
            set { SetProperty(ref canDeleteCommitment, value); }
        }

        private bool canAnalyseSelectedCommitment = false;

        public bool CanAnalyseSelectedCommitment
        {
            get { return this.canAnalyseSelectedCommitment; }
            set { SetProperty(ref canAnalyseSelectedCommitment, value); }
        }

        private bool canAnalyseSelection = false;

        public bool CanAnalyseSelection
        {
            get { return this.canAnalyseSelection; }
            set { SetProperty(ref canAnalyseSelection, value); }
        }

        private bool canAnalyse = true;

        public bool CanAnalyse
        {
            get { return this.canAnalyse; }
            set { SetProperty(ref canAnalyse, value); }
        }

  


        private ObservableCollection<InvestorCommitment> commitments;

        public ObservableCollection<InvestorCommitment> Commitments
        {
            get { return this.commitments; }
            set { SetProperty(ref commitments, value); }
        }

        public object OnConfirmation { get; }

        public ShowInvestorCommitmentsViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            NotificationRequest = new InteractionRequest<INotification>();
            ConfirmationRequest = new InteractionRequest<IConfirmation>();
            NewCommitmentCommand = new DelegateCommand(OnNewCommitment);
            EditCommitmentCommand = new DelegateCommand(OnEditCommitment).ObservesCanExecute(() => CanEditCommitment);
            DeleteCommitmentCommand = new DelegateCommand(OnDeleteCommitment).ObservesCanExecute(() => CanDeleteCommitment);
            GridMouseDoubleClickCommand = new DelegateCommand(OnGridMouseDoubleClick);
            GridViewSelectionChangedCommand = new DelegateCommand<object>(OnGridViewSelectionChanged);
            AnalyseAllCommitmentsCommand = new DelegateCommand(OnAnalyseAllCommitments).ObservesCanExecute(() => CanAnalyse);
            AnalyseSelectedCommitmentCommand = new DelegateCommand(OnAnalyseSelectedCommitment).ObservesCanExecute(() => CanAnalyseSelectedCommitment);
            AnalyseSelectionCommand = new DelegateCommand(OnAnalyseSelection).ObservesCanExecute(() => CanAnalyseSelection);
            ShowCashFlowsCommand = new DelegateCommand(OnShowCashFlows).ObservesCanExecute(() => CanEditCommitment);
            ShowPcapsCommand = new DelegateCommand(OnShowPcaps).ObservesCanExecute(() => CanEditCommitment);
            ShowPeFundCommand = new DelegateCommand(OnGridMouseDoubleClick).ObservesCanExecute(() => CanEditCommitment);
            CreateCommitmentReportCommand = new DelegateCommand(OnCreateCommitmentReport).ObservesCanExecute(() => CanAnalyse);
           
        }

        

        private void OnShowPcaps()
        {
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Commitment", SelectedCommitment);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ShowInvestorPcap, parameter);
        }

        private void OnCreateCommitmentReport()
        {

            NotificationRequest.Raise(new Notification
            {
                Title = "Report ",
                Content = $"Report Commitments"
            });
        }

        private void OnShowCashFlows()
        {
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Commitment", SelectedCommitment);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ShowInvestorCashflows, parameter);
        }

        private void OnAnalyseSelection()
        {
            AnalyzingInformation info = new AnalyzingInformation()
            {
                AllCommitments = new List<InvestorCommitment>(Commitments),
                SelectedCommitments = new List<InvestorCommitment>(selectedCommitments)
            };

            eventAggregator.GetEvent<AnalyzingInformationEvent>().Publish(info);
        }

        private void OnAnalyseSelectedCommitment()
        {
            AnalyzingInformation info = new AnalyzingInformation()
            {
                AllCommitments = new List<InvestorCommitment>(Commitments),
                SelectedCommitments = new List<InvestorCommitment>()
            };
            info.SelectedCommitments.Add(SelectedCommitment);
            eventAggregator.GetEvent<AnalyzingInformationEvent>().Publish(info);
        }

        private void OnAnalyseAllCommitments()
        {
            AnalyzingInformation info = new AnalyzingInformation()
            {
                AllCommitments = new List<InvestorCommitment>(Commitments),
                SelectedCommitments=new List<InvestorCommitment>(Commitments)
            };

            eventAggregator.GetEvent<AnalyzingInformationEvent>().Publish(info);
        }

        private void OnGridViewSelectionChanged(object items)
        {
            selectedCommitments = new List<InvestorCommitment>();
            foreach(object item in (ObservableCollection<object>)items)
            {
                selectedCommitments.Add(item as InvestorCommitment);
            }
          
            //{
            //    selectedCommitments.Add((InvestorCommitment)item);
            //}
            if (selectedCommitments.Count>1)
            {
                canEditCommitment = false;
                canDeleteCommitment = false;
                CanAnalyseSelection = true;
                CanAnalyse = true;
                CanAnalyseSelectedCommitment = false;
            }
            else if (selectedCommitments.Count==0)
            {
                CanAnalyse = false;
                CanAnalyseSelectedCommitment = false;
                CanAnalyseSelection = false;
            }
            else
            {
                CanAnalyse = true;
                CanAnalyseSelection = true;
                CanAnalyseSelectedCommitment = true;
            }
        }

        private void OnDeleteCommitment()
        {
            ConfirmationRequest.Raise(new Confirmation
            {
                Title = "delete ",
                Content = $"Delete Commitment on {SelectedCommitment.PeFund.FundShortName}"
            }, (OnConfirmDeleteCommitment));
        }

        private void OnConfirmDeleteCommitment(IConfirmation confirm)
        {
            if (!confirm.Confirmed) return;

            // delete Commitment

            try
            {
                investorAccess.DeleteInvestorCommitment(SelectedCommitment);
                eventAggregator.GetEvent<StatusBarEvent>().Publish($"Das Commitment für den Fonds {SelectedCommitment.PeFund.FundShortName} wurde gelöscht.");

                Commitments.Remove(SelectedCommitment);                
            }
            catch (Exception ex)
            {
                NotificationRequest.Raise(new Notification
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = $"Das Commitment für den Fonds {SelectedCommitment.PeFund.FundShortName} konnte nicht gelöscht werden. " + ex.Message
                });
            }
        }

        private void OnEditCommitment()
        {
            InvokeEditInvestorCommitment parameter = new InvokeEditInvestorCommitment();
            parameter.investorCommitment = SelectedCommitment;
            ObservableCollection<PeFund> fund = new ObservableCollection<PeFund>();
            fund.Add(SelectedCommitment.PeFund);
            parameter.availableFunds = fund;
            parameter.bankAccounts = new ObservableCollection<BankAccount>(Investor.BankAccounts);

            eventAggregator.GetEvent<InvokeEditInvestorCommitmentEvent>().Publish(parameter);
            EditCommitmentWindowState = WindowState.Open;
            
        }

        private void OnNewCommitment()
        {
            if(SelectedCommitment == null)
            {
                SelectedCommitment = new InvestorCommitment()
                {
                    InvestorId = Investor.Id
                };
            }
            InvokeEditInvestorCommitment parameter = new InvokeEditInvestorCommitment();
            parameter.investorCommitment = new InvestorCommitment()
            {
                InvestorId = SelectedCommitment.InvestorId
            };    
            parameter.availableFunds = availableFunds;
            parameter.bankAccounts = new ObservableCollection<BankAccount>(Investor.BankAccounts);

            eventAggregator.GetEvent<InvokeEditInvestorCommitmentEvent>().Publish(parameter);
            EditCommitmentWindowState = WindowState.Open;
        }

        private void OnGridMouseDoubleClick()
        {
            if (SelectedCommitment == null) return;

            PeFund entirePeFund = PefundAccess.GetPeFundById(SelectedCommitment.PeFundId);

            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Fund", entirePeFund);
            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.PeFundDetail, parameter);
        }

       

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            Investor parameter = navigationContext.Parameters["Investor"] as Investor;
            if (parameter.Id == Investor.Id) return true;
            return false;
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Investor parameter = navigationContext.Parameters["Investor"] as Investor;
            if (parameter == null) return;
            Investor = parameter;
            Commitments = new ObservableCollection<InvestorCommitment> (investorAccess.GetCommitmentsForInvestor(Investor.Id));
            AddPeResults();
            if (string.IsNullOrEmpty( Investor.InvestorHqTrustAccount))
            {
                TabTitle = $"{Investor.InvestorReference} ({Commitments.Count} Commitments)";
            }
            else
            {
                TabTitle = $"{Investor.InvestorHqTrustAccount} ({Commitments.Count} Commitments)";
            }
            RaisePropertyChanged("Investor");
            SetAvailableFunds();
            token = eventAggregator.GetEvent<EditInvestorCommitmentResponseEvent>().Subscribe(OnEditInvestorCommitmentResponse);

        }
        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            eventAggregator.GetEvent<EditInvestorCommitmentResponseEvent>().Unsubscribe(token);
        }

        private void OnEditInvestorCommitmentResponse(EditInvestorCommitmentResponse response)
        {
            switch (response.ActionType)
            {
                case "OK":
                    {
                        if (response.editedCommitment.Id == 0)
                        {
                            Commitments.Add(response.editedCommitment);
                            try
                            {
                                investorAccess.UpdateInvestorCommitments(response.editedCommitment);
                            }
                            catch (Exception ex)
                            {
                                NotificationRequest.Raise(new Notification
                                {
                                    Title = "Commitment ",
                                    Content = $"Fehler beim Update der Datenbank: {ex.Message}"
                                });
                                return;
                            }
                        }
                        else
                        {
                            try
                            {
                                investorAccess.UpdateInvestorCommitments(SelectedCommitment);
                            }
                            catch (Exception ex)
                            {
                                NotificationRequest.Raise(new Notification
                                {
                                    Title = "Commitment ",
                                    Content = $"Fehler beim Update der Datenbank: {ex.Message}"
                                });
                                return;
                            }
                        }
                        string message = $"Commitment für {SelectedCommitment.PeFund.FundShortName} wurde eingefügt bzw. geändert";
                        eventAggregator.GetEvent<StatusBarEvent>().Publish(message);
                        break;
                    }
                case "Cancel":
                    {
                        NotificationRequest.Raise(new Notification
                        {
                            Title = "Commitment ",
                            Content = $"Edit Response was Cancel"
                        });
                        string message = $"Änderungen wurden nicht gespeichert.";
                        eventAggregator.GetEvent<StatusBarEvent>().Publish(message);
                        break;
                    }
            }
            EditCommitmentWindowState = WindowState.Closed;
        }

        private void SetAvailableFunds()
        {
            var task = LoadPefundsAsync();

        }
        private async Task LoadPefundsAsync()
        {           
            availableFunds = new ObservableCollection<PeFund>(await PefundAccess.GetAllPefundsAsync());

            // remove all funds where the investor is already invested in from available funds
            // this ensures that a new commitment can't be made for an already existing fund

            if (Commitments != null && Commitments.Count>0)
            {
                foreach(InvestorCommitment commitment in Commitments)
                {
                    availableFunds.Remove(commitment.PeFund);
                }
            }
        }

        /// <summary>
        /// This routine calculates capital calls, distribution and other performance numbers and adds information to Commitments
        /// </summary>
        private void AddPeResults()
        {
            PeFundResults results;
            InvestorCashFlowComparer cashFlowComparer = new InvestorCashFlowComparer();

            foreach(InvestorCommitment commitment in Commitments)
            {
                InvestorCashFlowComparer comparer = new InvestorCashFlowComparer();
                commitment.InvestorCashFlows.Sort(comparer);
                try
                {
                    results = new PeFundResults(commitment, null, null);
                    commitment.Dpi = results.Dpi;
                    commitment.Irr = results.Irr;
                    commitment.OpenCommitment = results.OpenCommitment;
                    commitment.TotalCapitalCalls = results.AmountCalled;
                    commitment.TotalDistributions = results.AmountDistributed;
                    commitment.Tvpi = results.Tvpi;
                    commitment.LastNav = results.ValuationFundCurrency;
                }
                catch (Exception ex)
                {
                    Notification notification = new Notification
                    {
                        Title = ApplicationNames.NotificationTitle,
                        Content = ex.Message
                    };
                    NotificationRequest.Raise(notification);
                }                            
            }
            
        }
    }
}
