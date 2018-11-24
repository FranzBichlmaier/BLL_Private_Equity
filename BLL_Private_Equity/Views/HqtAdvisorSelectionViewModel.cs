using BLL_DataAccess;
using BLL_DataModels;
using BLL_Private_Equity.Events;
using BLL_Infrastructure;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace BLL_Private_Equity.Views
{
    public class HqtAdvisorSelectionViewModel: BLL_Prism.HqtBindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;

        public ICommand LoadedCommand { get; set; }
        public ICommand AdvisorSelectedCommand { get; set; }

        private ObservableCollection<Advisor> advisors;


        private Advisor selectedAdvisor;

        public Advisor SelectedAdvisor
        {
            get { return this.selectedAdvisor; }
            set { SetProperty(ref selectedAdvisor, value); }
        }

        private ICollectionView advisorList;

        public ICollectionView AdvisorList
        {
            get { return this.advisorList; }
            set { SetProperty(ref advisorList, value); }
        }

        private bool advisorsLoading;

        public bool AdvisorsLoading
        {
            get { return this.advisorsLoading; }
            set { SetProperty(ref advisorsLoading, value); }
        }

        public HqtAdvisorSelectionViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;

            LoadedCommand = new DelegateCommand(OnLoaded);
            AdvisorSelectedCommand = new DelegateCommand(OnAdvisorSelected);

            eventAggregator.GetEvent<AdvisorUpdatedEvent>().Subscribe(OnAdvisorUpdated);
        }

        private void OnAdvisorUpdated(string obj)
        {
            var task = LoadClientAdvisorsAsync();
        }

        private void OnAdvisorSelected()
        {
            if (SelectedAdvisor == null) return;
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Advisor", SelectedAdvisor);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.HqtAdvisorDetail, parameter);
        }

        private void OnLoaded()
        {
            var task = LoadClientAdvisorsAsync();
        }

        private async Task LoadClientAdvisorsAsync()
        {
            AdvisorsLoading = true;
            advisors = new ObservableCollection<Advisor>(await AdvisorAccess.GetAllHqtAdvisorsAsync());

            AdvisorList = CollectionViewSource.GetDefaultView(advisors);

            SortDescription sortDescription = new SortDescription()
            {
                Direction = ListSortDirection.Ascending,
                PropertyName = "FullName"
            };

            AdvisorList.SortDescriptions.Add(sortDescription);
            AdvisorsLoading = false;
        }
    }
}
