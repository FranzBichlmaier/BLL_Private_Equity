using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BLL_DataAccess;
using BLL_DataModels;
using BLL_Infrastructure;
using BLL_Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using BLL_Private_Equity.Events;

namespace BLL_Private_Equity.Views
{
    public class ClientAdvisorDetailViewModel: HqtBindableBase
    {
        private ClientAdvisor clientAdvisor;
        private ClientAdvisor copyOfClientAdvisor;
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;

        private bool canUserDeleteClientAdvisor;

        public bool CanUserDeleteClientAdvisor
        {
            get { return this.canUserDeleteClientAdvisor; }
            set { SetProperty(ref canUserDeleteClientAdvisor, value); }
        }
        
        public ICommand SaveChangesCommand { get; set; }
        public ICommand CancelChangesCommand { get; set; }
        public ICommand RemoveItemCommand { get; set; }
        public ICommand ShowInvestorsCommand { get; set; }
        public InteractionRequest<INotification> NotificationRequest { get; set; }

        public ClientAdvisor ClientAdvisor
        {
            get { return this.clientAdvisor; }
            set { SetProperty(ref clientAdvisor, value); }
        }

        public IRegionManager RegionManager { get; set; }

        public ClientAdvisorDetailViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;

            SaveChangesCommand = new DelegateCommand(OnSaveChanges);
            CancelChangesCommand = new DelegateCommand(OnCancelChanges);
            RemoveItemCommand = new DelegateCommand(OnRemoveItem).ObservesCanExecute(() => CanUserDeleteClientAdvisor);
            ShowInvestorsCommand = new DelegateCommand(OnShowInvestors);

            NotificationRequest = new InteractionRequest<INotification>();
        }

        private void OnShowInvestors()
        {
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("ClientAdvisorId", ClientAdvisor.Id);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ClientAdvisorInvestors, parameter);

        }

        private void OnRemoveItem()
        {
            try
            {
                AdvisorAccess.RemoveClientAdvisor(ClientAdvisor);
                eventAggregator.GetEvent<ClientAdvisorUpdateEvent>().Publish("Changed");
            }
            catch (Exception ex)
            {
                NotificationRequest.Raise(new Notification()
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = $"{ex.Message}"
                });
            }
        }

        private void OnCancelChanges()
        {
            ClientAdvisor = new ClientAdvisor(copyOfClientAdvisor);
        }

        private void OnSaveChanges()
        {
            try
            {
                AdvisorAccess.UpdateClientAdvisor(ClientAdvisor);
                copyOfClientAdvisor = new ClientAdvisor(ClientAdvisor);
                eventAggregator.GetEvent<ClientAdvisorUpdateEvent>().Publish("Changed");
            }
            catch (Exception ex)
            {
                NotificationRequest.Raise(new Notification()
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = $"{ex.Message}"
                });
            }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            ClientAdvisor i = navigationContext.Parameters["ClientAdvisor"] as ClientAdvisor;
            if (ClientAdvisor == null) return true;
            if (i.Id == ClientAdvisor.Id) return true;
            return false;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            ClientAdvisor = navigationContext.Parameters["ClientAdvisor"] as ClientAdvisor;           

            copyOfClientAdvisor = new ClientAdvisor(ClientAdvisor);
            TabTitle = ClientAdvisor.DisplayName;
        }
    }
}
