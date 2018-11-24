using BLL_DataAccess;
using BLL_DataModels;
using BLL_Private_Equity.Berechnungen;
using BLL_Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Telerik.Windows.Controls.GridView;
using Xceed.Wpf.Toolkit;

namespace BLL_Private_Equity.Views
{
    public class EditInvestorSplitViewModel: HqtBindableBase
    {


        private CashFlowInformation cashFlowInformation;
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private InvestorAccess investorAccess = new InvestorAccess();

        private ObservableCollection<InvestorCommitment> investorCommitments;
        private int newInvestorId;

        private ICollectionView newInvestorCommitments;

        public ICollectionView NewInvestorCommitments
        {
            get { return this.newInvestorCommitments; }
            set { SetProperty(ref newInvestorCommitments, value); }
        }


        private List<Investor> investors;

        public List<Investor> Investors
        {
            get { return this.investors; }
            set { SetProperty(ref investors, value); }
        }


        private WindowState showInvestorCommitments = WindowState.Closed;

        public WindowState ShowInvestorCommitments
        {
            get { return this.showInvestorCommitments; }
            set { SetProperty(ref showInvestorCommitments, value); }
        }

        private double differenceCapitalCall;

        public double DifferenceCapitalCall
        {
            get { return this.differenceCapitalCall; }
            set { SetProperty(ref differenceCapitalCall, value); }
        }

        private double differenceExpenses;

        public double DifferenceExpenses
        {
            get { return this.differenceExpenses; }
            set { SetProperty(ref differenceExpenses, value); }
        }

        private double differenceLookback;

        public double DifferenceLookback
        {
            get { return this.differenceLookback; }
            set { SetProperty(ref differenceLookback, value); }
        }

        private double differenceDistribution;

        public double DifferenceDistribution
        {
            get { return this.differenceDistribution; }
            set { SetProperty(ref differenceDistribution, value); }
        }

        private double differenceReturnOfCapital;

        public double DifferenceReturnOfCapital
        {
            get { return this.differenceReturnOfCapital; }
            set { SetProperty(ref differenceReturnOfCapital, value); }
        }


        private double differenceCapitalGain;

        public double DifferenceCapitalGain
        {
            get { return this.differenceCapitalGain; }
            set { SetProperty(ref differenceCapitalGain, value); }
        }


        private double differenceDividends;

        public double DifferenceDividends
        {
            get { return this.differenceDividends; }
            set { SetProperty(ref differenceDividends, value); }
        }


        private double differenceWithholdingTax;

        public double DifferenceWithholdingTax
        {
            get { return this.differenceWithholdingTax; }
            set { SetProperty(ref differenceWithholdingTax, value); }
        }

        private double differenceRecallable;

        public double DifferenceRecallable
        {
            get { return this.differenceRecallable; }
            set { SetProperty(ref differenceRecallable, value); }
        }

        private bool isCapitalCall;

        public bool IsCapitalCall
        {
            get { return this.isCapitalCall; }
            set { SetProperty(ref isCapitalCall, value); }
        }

        private bool isDistribution;

        public bool IsDistribution
        {
            get { return this.isDistribution; }
            set { SetProperty(ref isDistribution, value); }
        }

        private bool canGotoNextStep = false;

        public bool CanGotoNextStep
        {
            get { return this.canGotoNextStep; }
            set { SetProperty(ref canGotoNextStep, value); }
        }


        private string fundHeader = string.Empty;

        public string FundHeader
        {
            get { return this.fundHeader; }
            set { SetProperty(ref fundHeader, value); }
        }

        private CashFlowDetail selectedItem;

        public CashFlowDetail SelectedItem
        {
            get { return this.selectedItem; }
            set { SetProperty(ref selectedItem, value); }
        }
        public InteractionRequest<INotification> NotificationRequest { get; set; }
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }
        public ICommand AutomaticSplitCommand { get; set; }
        public ICommand CellEditEndedCommand { get; set; }
        public ICommand ReCalculateCommand { get; set; }
        public ICommand GotoNextStepCommand { get; set; }
        public ICommand NewInvestorAddedCommand { get; set; }
        public ICommand RowValidatingCommand { get; set; }
        public ICommand SelectedCellsChangedCommand { get; set; }
        public ICommand SelectInvestorCommitmentCommand { get; set; }
       

        public CashFlowInformation CashFlowInformation
        {
            get { return this.cashFlowInformation; }
            set { SetProperty(ref cashFlowInformation, value); }
        }

        

        public EditInvestorSplitViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;

            NotificationRequest = new InteractionRequest<INotification>();
            ConfirmationRequest = new InteractionRequest<IConfirmation>();

            AutomaticSplitCommand = new DelegateCommand(OnAutomaticSplit);
            CellEditEndedCommand = new DelegateCommand(OnCellEditEnded);
            ReCalculateCommand = new DelegateCommand(OnReCalculate);
            GotoNextStepCommand = new DelegateCommand(OnGotoNextStep).ObservesCanExecute(() => CanGotoNextStep);
            NewInvestorAddedCommand = new DelegateCommand(OnNewInvestorAdded);
            RowValidatingCommand = new DelegateCommand<Telerik.Windows.Controls.GridViewRowValidatingEventArgs>(OnRowValidating);
            SelectedCellsChangedCommand = new DelegateCommand<GridViewSelectedCellsChangedEventArgs>(OnSelectedCellsChanged);
            SelectInvestorCommitmentCommand = new DelegateCommand(OnSelectInvestorCommitment);
        }

        private void OnSelectInvestorCommitment()
        {
            if (NewInvestorCommitments.CurrentItem == null) return;
            ShowInvestorCommitments = WindowState.Closed;

            foreach(CashFlowDetail detail in CashFlowInformation.InvestorDetails)
            {
                if (detail.InvestorId != newInvestorId) continue;
                InvestorCommitment commitment = NewInvestorCommitments.CurrentItem as InvestorCommitment;
                detail.InvestorCommitmentId = commitment.Id;
                break;
            }
        }

        private void OnSelectedCellsChanged(GridViewSelectedCellsChangedEventArgs obj)
        {
            if (obj.AddedCells.Count == 0) return;

            Telerik.Windows.Controls.GridViewCellInfo cell = obj.AddedCells[0];
            SelectedItem = cell.Item as CashFlowDetail;
        }

        private void OnRowValidating(Telerik.Windows.Controls.GridViewRowValidatingEventArgs e)
        {
            // check row whether distribution is consistent
            CashFlowDetail detail = (CashFlowDetail)e.Row.Item;
            if (detail.CashFlowAmountDistribution == 0) return; //no checks in case of a capital call
            double difference = (detail.CashFlowAmountDistribution - detail.ReturnOfCapital - detail.CapitalGain - detail.Dividends - detail.RecallableAmount - detail.WithholdingTax);
            if (Math.Round(difference, 2) == 0) return;
            e.ValidationResults.Add(new Telerik.Windows.Controls.GridViewCellValidationResult()
            {
                ErrorMessage = $"Die Ausschüttungskomponenten entsprechen nicht dem Ausschüttungsbetrag. Die Differenz beträgt {difference:n2}."
            });
        }

     

        private void OnNewInvestorAdded()
        {
            CashFlowDetail detail = new CashFlowDetail();
            CashFlowInformation.InvestorDetails.Add(detail);
            SelectedItem = detail;   
            
            
        }

        private void OnGotoNextStep()
        {
            CashFlowInformation.InvestorDetailsEntered = true;
            Events.PrepareCashFlow prepareCashFlow = new Events.PrepareCashFlow()
            {
                cfInfo = CashFlowInformation
            };
            eventAggregator.GetEvent<Events.PrepareCashFlowEvent>().Publish(prepareCashFlow);
        }

        private void OnReCalculate()
        {
            OnAutomaticSplit();
        }

        private void OnCellEditEnded()
        {
          
            // calculate all columns and identify differences
            double totalCommitment = 0;
            CashFlowDetail sums = CashFlowInformation.DetailSummary;
            DifferenceCapitalCall = Math.Abs(sums.CashFlowAmountCall);
            DifferenceExpenses = Math.Abs(sums.PartnershipExpenses);
            DifferenceLookback = Math.Abs(sums.LookbackInterests);
            DifferenceDistribution = Math.Abs(sums.CashFlowAmountDistribution);
            DifferenceReturnOfCapital = Math.Abs(sums.ReturnOfCapital);
            DifferenceCapitalGain = Math.Abs(sums.CapitalGain);
            DifferenceDividends = Math.Abs(sums.Dividends);
            DifferenceWithholdingTax = Math.Abs(sums.WithholdingTax);
            DifferenceRecallable = Math.Abs(sums.RecallableAmount);
            foreach (CashFlowDetail detail in CashFlowInformation.InvestorDetails)
            {   
                if (detail.InvestorId!= 0 && detail.Investor ==null)
                {
                    detail.Investor = Investors.FirstOrDefault(i => i.Id == detail.InvestorId);
                    if (detail.Investor != null)
                    {
                        detail.Reference = detail.Investor.InvestorReference;
                        investorCommitments = new ObservableCollection<InvestorCommitment>(investorAccess.GetCommitmentsForInvestor(detail.InvestorId));
                        NewInvestorCommitments = CollectionViewSource.GetDefaultView(investorCommitments);
                        newInvestorId = detail.InvestorId;
                        ShowInvestorCommitments = WindowState.Open;
                    }
                }
                DifferenceCapitalCall -= Math.Abs(detail.CashFlowAmountCall);              
                DifferenceExpenses -= Math.Abs(detail.PartnershipExpenses);                
                DifferenceLookback -= Math.Abs(detail.LookbackInterests);
                totalCommitment += Math.Abs(detail.CommitmentAmount);
                DifferenceDistribution -= Math.Abs(detail.CashFlowAmountDistribution);
                DifferenceReturnOfCapital -= Math.Abs(detail.ReturnOfCapital);
                DifferenceCapitalGain -= Math.Abs(detail.CapitalGain);
                DifferenceDividends -= Math.Abs(detail.Dividends);
                DifferenceWithholdingTax -= Math.Abs(detail.WithholdingTax);
                DifferenceRecallable -= Math.Abs(detail.RecallableAmount);
            }
            DifferenceCapitalCall = Math.Round(DifferenceCapitalCall, 2);
            DifferenceExpenses = Math.Round(DifferenceExpenses, 2);
            DifferenceLookback = Math.Round(DifferenceLookback, 2);
            DifferenceDistribution = Math.Round(DifferenceDistribution,2);
            DifferenceReturnOfCapital = Math.Round(DifferenceReturnOfCapital,2);
            DifferenceCapitalGain = Math.Round(DifferenceCapitalGain,2);
            DifferenceDividends = Math.Round(DifferenceDividends,2);
            DifferenceWithholdingTax = Math.Round(DifferenceWithholdingTax,2);
            DifferenceRecallable = Math.Round(DifferenceRecallable,2);
            sums.CommitmentAmount = totalCommitment;
            FundHeader = $"{CashFlowInformation.Fund.FundShortName} ({CashFlowInformation.DetailSummary.CommitmentAmount:n0})";
            if (DifferenceCapitalCall == 0 && DifferenceExpenses == 0 && DifferenceLookback == 0) CanGotoNextStep = true; else CanGotoNextStep = false;
        }

        private void OnAutomaticSplit()
        {
            // the total Cash Flow (DetailSummary) will be split according commitments of investors (InvestorDetails)
            CashFlowDetail sums = CashFlowInformation.DetailSummary;
            DifferenceCapitalCall = Math.Abs(sums.CashFlowAmountCall);
            DifferenceExpenses = Math.Abs(sums.PartnershipExpenses);
            DifferenceLookback = Math.Abs(sums.LookbackInterests);
            DifferenceDistribution = Math.Abs(sums.CashFlowAmountDistribution);
            DifferenceReturnOfCapital = Math.Abs(sums.ReturnOfCapital);
            DifferenceCapitalGain = Math.Abs(sums.CapitalGain);
            DifferenceDividends = Math.Abs(sums.Dividends);
            DifferenceWithholdingTax = Math.Abs(sums.WithholdingTax);
            DifferenceRecallable = Math.Abs(sums.RecallableAmount);

            foreach (CashFlowDetail detail in CashFlowInformation.InvestorDetails)
            {                
                detail.CashFlowAmountCall = Math.Round(sums.CashFlowAmountCall / sums.CommitmentAmount * detail.CommitmentAmount, 2);
                DifferenceCapitalCall -= Math.Abs(detail.CashFlowAmountCall);
                detail.PartnershipExpenses = Math.Round(sums.PartnershipExpenses / sums.CommitmentAmount * detail.CommitmentAmount, 2);
                DifferenceExpenses -= Math.Abs(detail.PartnershipExpenses);
                detail.LookbackInterests = Math.Round(sums.LookbackInterests / sums.CommitmentAmount * detail.CommitmentAmount, 2);
                DifferenceLookback -= Math.Abs(detail.LookbackInterests);
                detail.CashFlowAmountDistribution = Math.Round(sums.CashFlowAmountDistribution / sums.CommitmentAmount * detail.CommitmentAmount, 2);
                DifferenceDistribution -= Math.Abs(detail.CashFlowAmountDistribution);
                detail.ReturnOfCapital = Math.Round(sums.ReturnOfCapital / sums.CommitmentAmount * detail.CommitmentAmount, 2);
                DifferenceReturnOfCapital -= Math.Abs(detail.ReturnOfCapital);
                detail.CapitalGain = Math.Round(sums.CapitalGain / sums.CommitmentAmount * detail.CommitmentAmount, 2);
                DifferenceCapitalGain -= Math.Abs(detail.CapitalGain);
                detail.Dividends = Math.Round(sums.Dividends / sums.CommitmentAmount * detail.CommitmentAmount, 2);
                DifferenceDividends -= Math.Abs(detail.Dividends);
                detail.WithholdingTax = Math.Round(sums.WithholdingTax / sums.CommitmentAmount * detail.CommitmentAmount, 2);
                DifferenceWithholdingTax -= Math.Abs(detail.WithholdingTax);
                detail.RecallableAmount = Math.Round(sums.RecallableAmount / sums.CommitmentAmount * detail.CommitmentAmount, 2);
                DifferenceRecallable -= Math.Abs(detail.RecallableAmount);
                // set CashFlowAmount
                if (detail.CashFlowAmountCall != 0) detail.CashFlowAmount = detail.CashFlowAmountCall;
                else detail.CashFlowAmount = detail.CashFlowAmountDistribution;
            }

            FundHeader = $"{CashFlowInformation.Fund.FundShortName} ({CashFlowInformation.DetailSummary.CommitmentAmount:n0})";

            // Rounding Differences are allocated to the first investor

            CashFlowInformation.InvestorDetails[0].CashFlowAmountCall += DifferenceCapitalCall;
            CashFlowInformation.InvestorDetails[0].LookbackInterests += DifferenceLookback;
            CashFlowInformation.InvestorDetails[0].PartnershipExpenses += DifferenceExpenses;
            CashFlowInformation.InvestorDetails[0].CashFlowAmountDistribution += DifferenceDistribution;
            CashFlowInformation.InvestorDetails[0].ReturnOfCapital += DifferenceReturnOfCapital;
            CashFlowInformation.InvestorDetails[0].CapitalGain += DifferenceCapitalGain;
            CashFlowInformation.InvestorDetails[0].Dividends += DifferenceDividends;
            CashFlowInformation.InvestorDetails[0].WithholdingTax += DifferenceWithholdingTax;
            CashFlowInformation.InvestorDetails[0].RecallableAmount += DifferenceRecallable;

            DifferenceExpenses = 0;
            DifferenceLookback = 0;
            DifferenceCapitalCall = 0;
            DifferenceDistribution = 0;
            DifferenceReturnOfCapital = 0;
            DifferenceCapitalGain = 0;
            DifferenceDividends = 0;
            DifferenceWithholdingTax = 0;
            DifferenceRecallable = 0;
            CanGotoNextStep = true;

            if (sums.CashFlowAmountCall == 0) IsCapitalCall = false; else IsCapitalCall = true;
            if (sums.CashFlowAmountDistribution == 0) IsDistribution = false; else IsDistribution = true;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            CashFlowInformation = navigationContext.Parameters["Info"] as CashFlowInformation;
            RaisePropertyChanged("CashFlowInformation");

            OnAutomaticSplit();
            GetInvestors();
        }
        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        private async void GetInvestors()
        {
            investors = (List<Investor>)await (investorAccess.GetAllInvestorsAsync());
            Comparer.InvestorFullNameComparer comparer = new BLL_Private_Equity.Comparer.InvestorFullNameComparer();
            investors.Sort(comparer);
            Investors = investors.Where(i => i.Commitments.Count > 0).ToList();
            RaisePropertyChanged("Investors");
        }
    }
}
