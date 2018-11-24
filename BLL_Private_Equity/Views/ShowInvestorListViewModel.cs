using BLL_DataAccess;
using BLL_DataModels;
using BLL_Infrastructure;
using BLL_Prism;
using Prism.Commands;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using Telerik.Windows.Data;

namespace BLL_Private_Equity.Views
{
    class ShowInvestorListViewModel: HqtBindableBase
    {
        private InvestorAccess investorAccess = new InvestorAccess();
        public IEnumerable<EnumMemberViewModel> InvestorGroups { get; } = EnumDataSource.FromType<InvestorGroup>();

        private ObservableCollection<Investor> investors;
        public ICollectionView InvestorList { get; set; }


        private bool investorsLoading;
        private readonly IRegionManager regionManager;

        public bool InvestorsLoading
        {
            get { return this.investorsLoading; }
            set { SetProperty(ref investorsLoading, value); }
        }

        public DelegateCommand LoadedCommand { get; set; }
        public DelegateCommand ShowInvestorDetailsCommand { get; set; }
        public ShowInvestorListViewModel(IRegionManager regionManager)
        {
            TabTitle = "Investorenliste";
            LoadedCommand = new DelegateCommand(OnLoaded);
            ShowInvestorDetailsCommand = new DelegateCommand(OnShowInvestorDetails);
            this.regionManager = regionManager;
        }

        private void OnShowInvestorDetails()
        {
            if (InvestorList.CurrentItem == null) return;
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Investor", InvestorList.CurrentItem as Investor);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.InvestorDetails, parameter);
        }

        private void OnLoaded()
        {
            var task = LoadInvestorsAsync();
        }

        private async Task LoadInvestorsAsync()
        {
            InvestorsLoading = true;
            investors = new ObservableCollection<Investor>(await investorAccess.GetAllInvestorsAsync());


            InvestorList = CollectionViewSource.GetDefaultView(investors);



            SortDescription sortDescription = new SortDescription()
            {
                Direction = ListSortDirection.Ascending,
                PropertyName = "InvestorReference"
            };

            InvestorList.SortDescriptions.Add(sortDescription);
            InvestorsLoading = false;
            RaisePropertyChanged("InvestorList");

        }
    }
}
