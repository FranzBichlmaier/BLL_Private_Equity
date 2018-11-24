using BLL_Prism;
using Prism.Events;
using Prism.Regions;
using BLL_Private_Equity.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL_DataModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using BLL_Private_Equity.Berechnungen;
using BLL_DataAccess;
using Telerik.Windows.Data;
using DateTimeFunctions;
using BLL_Infrastructure;

namespace BLL_Private_Equity.Views
{
    public class AnalyzeSettingsViewModel: HqtBindableBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        public IEnumerable<EnumMemberViewModel> AnalyzingParameterPeriods { get; } = EnumDataSource.FromType<AnalyzingParameterPeriod>();

        private InvestorAccess investorAccess = new InvestorAccess();
        private DateDifferences dateFunctions = new DateDifferences();

        public IEnumerable<Investor> Investors { get; set; }
        private AnalyzingParameter   analyzingParameter;

        public AnalyzingParameter AnalyzingParameter
        {
            get { return this.analyzingParameter; }
            set { SetProperty(ref analyzingParameter, value); }
        }

        private ObservableCollection<InvestorCommitment> allCommitments;

        public ObservableCollection<InvestorCommitment> AllCommitments
        {
            get { return this.allCommitments; }
            set { SetProperty(ref allCommitments, value); }
        }

        private ObservableCollection<InvestorCommitment> selectedCommitments;

        public ObservableCollection<InvestorCommitment> SelectedCommitments
        {
            get { return this.selectedCommitments; }
            set { SetProperty(ref selectedCommitments, value); }
        }

        private Investor selectedInvestor;  

        public Investor SelectedInvestor
        {
            get { return this.selectedInvestor; }
            set { SetProperty(ref selectedInvestor, value); }
        }

        private DateTime displayDateStart;

        public DateTime DisplayDateStart
        {
            get { return this.displayDateStart; }
            set { SetProperty(ref displayDateStart, value); }
        }

        private DateTime displayDateEnd;

        public DateTime DisplayDateEnd
        {
            get { return this.displayDateEnd; }
            set { SetProperty(ref displayDateEnd, value); }
        }

        public ICommand InvestorSelectedCommand { get; set; }
        public ICommand ClearSelectionCommand { get; set; }
        public ICommand SelectAllCommand { get; set; }
        public ICommand RemoveFromListCommand { get; set; }
        public ICommand StartAnalyzingCommand { get; set; }
        public ICommand EndDateChangedCommand { get; set; }
        public ICommand StartDateChangedCommand { get; set; }

        public AnalyzeSettingsViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            InvestorSelectedCommand = new DelegateCommand(OnInvestorSelected);
            ClearSelectionCommand = new DelegateCommand(OnClearSelection);
            SelectAllCommand = new DelegateCommand(OnSelectAll);
            RemoveFromListCommand = new DelegateCommand(OnRemoveFromList);
            StartAnalyzingCommand = new DelegateCommand(OnStartAnalyzing);
            EndDateChangedCommand = new DelegateCommand<DateTime?>(OnEndDateChanged);
            StartDateChangedCommand = new DelegateCommand<DateTime?>(OnStartDateChanged);
            AllCommitments = new ObservableCollection<InvestorCommitment>();
            SelectedCommitments = new ObservableCollection<InvestorCommitment>();
            

            ReadInvestors();

            AnalyzingParameter = new AnalyzingParameter()
            {
                Commitments = new List<ExtendedCommitment>(),
                EndDate = dateFunctions.PreviousQuarter(DateTime.Now),
                StartDate = new DateTime(1994, 1, 1)
            };
            AnalyzingParameter.PropertyChanged += AnalyzingParameter_PropertyChanged;


            eventAggregator.GetEvent<AnalyzingInformationEvent>().Subscribe(OnAnalyzingChanged);
        }

        private void OnStartDateChanged(DateTime? startDate)
        {
            if (startDate == null) return;
            AnalyzingParameter.StartDate = dateFunctions.PreviousQuarter((DateTime)startDate);
        }

        private void OnEndDateChanged(DateTime? endDate)
        {
            if (endDate == null) return;
            AnalyzingParameter.EndDate = dateFunctions.NextQuarter((DateTime)endDate);
        }

        private void AnalyzingParameter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           
        }

        private void OnRemoveFromList()
        {
            // selected Investments are in SelectedCommitments
            // remove each Commitment within SelectedCommitments an remove it from AllCommitments
            // clear SelectedCommitments
            foreach(InvestorCommitment item in SelectedCommitments)
            {
                AllCommitments.Remove(item);
            }
            SelectedCommitments.Clear();
        }

        private void OnInvestorSelected()
        {
            if (SelectedInvestor == null) return;
            // Read all commitments for the Investor and add them to AllCommitments
            IEnumerable<InvestorCommitment> additionalCommitments = investorAccess.GetCommitmentsForInvestor(SelectedInvestor.Id);
            foreach(InvestorCommitment item in additionalCommitments)
            {
                AllCommitments.Add(item);
            }
            SetPeriodStartAndEnd();
        }

        private async void ReadInvestors()
        {
            Investors = await investorAccess.GetAllInvestorsAsync();
            // Add DisplayName as a combination of HqTrustAccount and Name to Investors
            foreach(Investor investor in Investors)
            {
                investor.DisplayName = investor.InvestorHqTrustAccount + " " + investor.IName.FullName + " " + investor.InvestorReference;
            }
            RaisePropertyChanged("Investors");
        }

        private void OnStartAnalyzing()
        {
            AnalyzingParameter.Commitments.Clear();
            foreach(InvestorCommitment item in SelectedCommitments)
            {
                ExtendedCommitment commitment = new ExtendedCommitment()
                {
                    Commitment = item
                };
                AnalyzingParameter.Commitments.Add(commitment);
            }
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Parameter", AnalyzingParameter);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.AnalyzeContainer, parameter);
        }

        private void OnSelectAll()
        {
            SelectedCommitments.Clear();
            foreach(InvestorCommitment c in AllCommitments)
            {
                SelectedCommitments.Add(c);
            }
        }

        private void OnClearSelection()
        {
            SelectedCommitments.Clear();
        }

        private void OnAnalyzingChanged(AnalyzingInformation obj)
        {
            AllCommitments.Clear();
            foreach (InvestorCommitment c in obj.AllCommitments)
            {
                AllCommitments.Add(c);
            }
            SelectedCommitments.Clear();
            foreach (InvestorCommitment c in obj.SelectedCommitments)
            {
                SelectedCommitments.Add(c);
            }
            SetPeriodStartAndEnd();
        }

        private void SetPeriodStartAndEnd()
        {
            DateTime minDate = DateTime.MaxValue;
            DateTime maxDate = DateTime.MinValue;

            foreach(InvestorCommitment item in AllCommitments)
            {
                if (item.InvestorCashFlows.Count == 0) continue;
                if (item.InvestorCashFlows.Min(c => c.EffectiveDate) < minDate) minDate = (item.InvestorCashFlows.Min(c => c.EffectiveDate));
                if (item.InvestorCashFlows.Max(c => c.EffectiveDate) > maxDate) maxDate = (item.InvestorCashFlows.Max(c => c.EffectiveDate));
            }
            DisplayDateStart = dateFunctions.PreviousQuarter(minDate);
            DisplayDateStart = DisplayDateStart.AddDays(1);
            DisplayDateEnd = dateFunctions.NextQuarter(maxDate);
            if (DisplayDateEnd > DateTime.Now) DisplayDateEnd = dateFunctions.PreviousQuarter(DateTime.Now);
            AnalyzingParameter.StartDate = DisplayDateStart;
            AnalyzingParameter.EndDate = DisplayDateEnd;
        }
    }
}
