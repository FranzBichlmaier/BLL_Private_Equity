using BLL_DataAccess;
using BLL_DataModels;
using BLL_Private_Equity.Events;
using BLL_Infrastructure;
using BLL_Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BLL_Private_Equity.Views
{
    public class PeFundSelectionViewModel: HqtBindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;

        public DelegateCommand LoadedCommand { get; set; }
        public DelegateCommand DeleteFilterCommand { get; set; }
        public DelegateCommand FilterCommand { get; set; }
        public DelegateCommand PefundSelectedCommand { get; set; }
        

        private ObservableCollection<PeFund> peFunds;

        private ICollectionView peFundList;

        public ICollectionView PeFundList
        {
            get { return this.peFundList; }
            set { SetProperty(ref peFundList, value); }
        }


        private PeFund selectedPefund;

        public PeFund SelectedPefund
        {
            get { return this.selectedPefund; }
            set { SetProperty(ref selectedPefund, value); }
        }

        private bool peFundsLoading = true;

        public bool PeFundsLoading
        {
            get { return this.peFundsLoading; }
            set { SetProperty(ref peFundsLoading, value); }
        }

        private string filterText;

        public string FilterText
        {
            get { return this.filterText; }
            set { SetProperty(ref filterText, value); }
        }


        public PeFundSelectionViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            LoadedCommand = new DelegateCommand(OnLoaded);
            DeleteFilterCommand = new DelegateCommand(OnDeleteFilter);
            FilterCommand = new DelegateCommand(OnFilter);
            PefundSelectedCommand = new DelegateCommand(OnPefundSelected);
            eventAggregator.GetEvent<PeFundCollectionActionEvent>().Subscribe(OnPeFundCollectionAction);
        }

        private void OnPefundSelected()
        {
            if (SelectedPefund == null) return;
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Fund", SelectedPefund);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.PeFundDetail, parameter);
        }

        private void OnFilter()
        {
            if (PeFundList.Filter == null)
            {
                PeFundList.Filter = FilterInvestor;
                return;
            }

            PeFundList.Refresh();
        }

        private bool FilterInvestor(object obj)
        {
            PeFund fund = obj as PeFund;
            if (fund.FundName != null && fund.FundName.ToLower().Contains(FilterText.ToLower())) return true;
            if (fund.FundLegalName != null && fund.FundLegalName.ToLower().Contains(FilterText.ToLower())) return true;
            if (fund.FundHqTrustNumber != null && fund.FundHqTrustNumber.ToLower().Contains(FilterText.ToLower())) return true;
            return false;
        }

        private void OnDeleteFilter()
        {
            // Remove Filter from InvestorList
            PeFundList.Filter = null;
            FilterText = string.Empty;
        }

        private void OnLoaded()
        {
            var task = LoadPefundsAsync();
        }

        private async Task LoadPefundsAsync()
        {
            PeFundsLoading = true;
            peFunds = new ObservableCollection<PeFund>(await PefundAccess.GetAllPefundsAsync());


            PeFundList = CollectionViewSource.GetDefaultView(peFunds);



            SortDescription sortDescription = new SortDescription()
            {
                Direction = ListSortDirection.Ascending,
                PropertyName = "FundName"
            };

            PeFundList.SortDescriptions.Add(sortDescription);
            PeFundsLoading = false;
            RaisePropertyChanged("PeFundList");

        }
        private void OnPeFundCollectionAction(PeFundCollectionAction obj)
        {
            if (peFunds==null)
            {
                var task =LoadPefundsAsync();
            }
            if (obj.action == CollectionAction.removed)
            {
                PeFund fund = peFunds.FirstOrDefault(i => i.Id == obj.fund.Id);
                peFunds.Remove(fund);
            }
            else if (obj.action == CollectionAction.added)
            {
                peFunds.Add(obj.fund);
            }
            else if (obj.action == CollectionAction.updated)
            {
                PeFund fund = peFunds.FirstOrDefault(i => i.Id == obj.fund.Id);
                fund.FundHqTrustNumber = obj.fund.FundHqTrustNumber;
                fund.FundName = obj.fund.FundName;
                fund.FundShortName = obj.fund.FundShortName;
                fund.FundLegalName = obj.fund.FundLegalName;
            }
            else if (obj.action == CollectionAction.reload)
            {
                var task = LoadPefundsAsync();
            }
        }
    }
}
