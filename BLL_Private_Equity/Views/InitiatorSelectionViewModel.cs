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
    class InitiatorSelectionViewModel: BLL_Prism.HqtBindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventaggregator;

        public ICommand LoadedCommand { get; set; }
        public ICommand InitiatorSelectedCommand { get; set; }
        private ObservableCollection<Initiator> initiators;

        private ICollectionView initiatorList;

        public ICollectionView InitiatorList
        {
            get { return this.initiatorList; }
            set { SetProperty(ref initiatorList, value); }
        }

        private Initiator selectedInitiator;

        public Initiator SelectedInitiator
        {
            get { return this.selectedInitiator; }
            set { SetProperty(ref selectedInitiator, value); }
        }

        private bool initiatorsLoading = false;

        public bool InitiatorsLoading
        {
            get { return this.initiatorsLoading; }
            set { SetProperty(ref initiatorsLoading, value); }
        }

        public InitiatorSelectionViewModel(IRegionManager regionManager, IEventAggregator eventaggregator)
        {
            this.regionManager = regionManager;
            this.eventaggregator = eventaggregator;

            LoadedCommand = new DelegateCommand(OnLoaded);
            InitiatorSelectedCommand = new DelegateCommand(OnInitiatorSelected);
            eventaggregator.GetEvent<InitiatorUpdatedEvent>().Subscribe(OnInitiatorsUpdated);
        }

        private void OnInitiatorsUpdated(string obj)
        {
            var task = LoadInitiatorsAsync();
        }

        private void OnInitiatorSelected()
        {
            if (SelectedInitiator == null) return;
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Initiator", SelectedInitiator);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.InitiatorDetail, parameter);
        }

        private void OnLoaded()
        {
            var task = LoadInitiatorsAsync();
        }

        private async Task LoadInitiatorsAsync()
        {
            InitiatorsLoading = true;
            initiators = new ObservableCollection<Initiator>(await PefundAccess.GetAllInitiatorsAsync());

            InitiatorList = CollectionViewSource.GetDefaultView(initiators);

            SortDescription sortDescription = new SortDescription()
            {
                Direction = ListSortDirection.Ascending,
                PropertyName = "InitiatorName"
            };

            InitiatorList.SortDescriptions.Add(sortDescription);
            InitiatorsLoading = false;
        }
    }
}
