using BLL_DataAccess;
using BLL_DataModels;
using BLL_Infrastructure;
using BLL_Prism;
using BLL_Private_Equity.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace BLL_Private_Equity.Views
{
    public class ClientAdvisorSelectionViewModel: HqtBindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventaggregator;

        public ICommand LoadedCommand { get; set; }
        public ICommand ClientAdvisorSelectedCommand { get; set; }
        private ObservableCollection<ClientAdvisor> clientAdvisors;

        private ICollectionView clientAdvisorList;

        public ICollectionView ClientAdvisorList
        {
            get { return this.clientAdvisorList; }
            set { SetProperty(ref clientAdvisorList, value); }
        }

        private ClientAdvisor selectedClientAdvisor;

        public ClientAdvisor SelectedClientAdvisor
        {
            get { return this.selectedClientAdvisor; }
            set { SetProperty(ref selectedClientAdvisor, value); }
        }

        private bool clientAdvisorsLoading = false;

        public bool ClientAdvisorsLoading
        {
            get { return this.clientAdvisorsLoading; }
            set { SetProperty(ref clientAdvisorsLoading, value); }
        }

        public ClientAdvisorSelectionViewModel(IRegionManager regionManager, IEventAggregator eventaggregator)
        {
            this.regionManager = regionManager;
            this.eventaggregator = eventaggregator;

            LoadedCommand = new DelegateCommand(OnLoaded);
            ClientAdvisorSelectedCommand = new DelegateCommand(OnClientAdvisorSelected);
            eventaggregator.GetEvent<ClientAdvisorUpdateEvent>().Subscribe(OnClientAdvisorsUpdated);
        }

        private void OnClientAdvisorsUpdated(string obj)
        {
            var task = LoadClientAdvisorsAsync();
        }

        private void OnClientAdvisorSelected()
        {
            if (SelectedClientAdvisor == null) return;
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("ClientAdvisor", SelectedClientAdvisor);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ClientAdvisorDetail, parameter);
        }

        private void OnLoaded()
        {
            var task = LoadClientAdvisorsAsync();
        }

        private async Task LoadClientAdvisorsAsync()
        {
            ClientAdvisorsLoading = true;
            clientAdvisors = new ObservableCollection<ClientAdvisor>(await AdvisorAccess.GetAllClientAdvisorsAsync());

            ClientAdvisorList = CollectionViewSource.GetDefaultView(clientAdvisors);

            SortDescription sortDescription = new SortDescription()
            {
                Direction = ListSortDirection.Ascending,
                PropertyName = "DisplayName"
            };

            ClientAdvisorList.SortDescriptions.Add(sortDescription);
            ClientAdvisorsLoading = false;
        }
    }
}
