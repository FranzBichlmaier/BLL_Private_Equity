using BLL_DataAccess;
using BLL_DataModels;
using BLL_Private_Equity.Events;
using BLL_Infrastructure;
using BLL_Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BLL_Private_Equity.Views
{
    public class InitiatorDetailViewModel: HqtBindableBase, IRegionManagerAware
    {

        private Initiator initiator;
        private Initiator copyOfInitiator;
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;

        private bool canUserDeleteInitiator;

        public bool CanUserDeleteInitiator
        {
            get { return this.canUserDeleteInitiator; }
            set { SetProperty(ref canUserDeleteInitiator, value); }
        }
        public List<Advisor> Advisors { get; set; } = new List<Advisor>(ComboboxLists.GetAdvisors());
        public ICommand SaveChangesCommand { get; set; }
        public ICommand CancelChangesCommand { get; set; }
        public ICommand RemoveItemCommand { get; set; }
        public InteractionRequest<INotification> NotificationRequest { get; set; }

        public Initiator Initiator
        {
            get { return this.initiator; }
            set { SetProperty(ref initiator, value); }
        }

        public IRegionManager RegionManager { get ; set; }

        public InitiatorDetailViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;

            SaveChangesCommand = new DelegateCommand(OnSaveChanges);
            CancelChangesCommand = new DelegateCommand(OnCancelChanges);
            RemoveItemCommand = new DelegateCommand(OnRemoveItem).ObservesCanExecute(() => CanUserDeleteInitiator);

            NotificationRequest = new InteractionRequest<INotification>();
        }

        private void OnRemoveItem()
        {
            try
            {
                PefundAccess.RemoveInitiator(Initiator);
                eventAggregator.GetEvent<InitiatorUpdatedEvent>().Publish("Changed");
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
            Initiator = new Initiator(copyOfInitiator);
        }

        private void OnSaveChanges()
        {
            try
            {
                PefundAccess.UpdateInitiator(Initiator);
                copyOfInitiator = new Initiator(Initiator);
                eventAggregator.GetEvent<InitiatorUpdatedEvent>().Publish("Changed");
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
            Initiator i = navigationContext.Parameters["Initiator"] as Initiator;
            if (Initiator == null) return true;
            if (i.Id == Initiator.Id) return true;
            return false;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Initiator  = navigationContext.Parameters["Initiator"] as Initiator;

            // It is only possible to remove a Initiator if it is not related to a fund
            if (Initiator.Id != 0)
            {
                if (PefundAccess.IsInitiatorUsed(Initiator.Id)) CanUserDeleteInitiator = false; else CanUserDeleteInitiator = true;
            }
            else CanUserDeleteInitiator = false;
            
            copyOfInitiator = new Initiator(Initiator);
            TabTitle = Initiator.InitiatorName;
        }
    }
}
