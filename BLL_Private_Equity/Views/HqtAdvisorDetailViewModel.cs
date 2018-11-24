using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BLL_DataAccess;
using BLL_DataModels;
using BLL_Private_Equity.Events;
using BLL_Infrastructure;
using BLL_Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;

namespace BLL_Private_Equity.Views
{
    public class HqtAdvisorDetailViewModel : HqtBindableBase, IRegionManagerAware
    {
        public IRegionManager RegionManager { get; set; }
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private Advisor copyOfAdvisor;

        public ICommand SaveChangesCommand { get; set; }
        public ICommand CancelChangesCommand { get; set; }
        public ICommand RemoveItemCommand { get; set; }
        public InteractionRequest<INotification> NotificationRequest { get; set; }

        public List<Advisor> ReportsToList { get; set; }


        private bool canRemoveAdvisor;

        public bool CanRemoveAdvisor
        {
            get { return this.canRemoveAdvisor; }
            set { SetProperty(ref canRemoveAdvisor, value); }
        }

        private Advisor advisor;

        public Advisor Advisor
        {
            get { return this.advisor; }
            set { SetProperty(ref advisor, value); }
        }

        public HqtAdvisorDetailViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;

            SaveChangesCommand = new DelegateCommand(OnSaveChanges);
            CancelChangesCommand = new DelegateCommand(OnCancelChanges);
            RemoveItemCommand = new DelegateCommand(OnRemoveItem).ObservesCanExecute(() => CanRemoveAdvisor);

            NotificationRequest = new InteractionRequest<INotification>();
        }

        private void OnRemoveItem()
        {
            try
            {
                AdvisorAccess.RemoveAdvisor(Advisor);
                eventAggregator.GetEvent<AdvisorUpdatedEvent>().Publish("Changed");
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
            Advisor = copyOfAdvisor;
        }

        private void OnSaveChanges()
        {
            try
            {
                AdvisorAccess.UpdateAdvisor(Advisor);
                copyOfAdvisor = new Advisor(Advisor);
                eventAggregator.GetEvent<AdvisorUpdatedEvent>().Publish("Changed");
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
            if (Advisor == null) return true;
            Advisor newAdvisor = navigationContext.Parameters["Advisor"] as Advisor;
            if (newAdvisor.Id == Advisor.Id) return true;
            return false;
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Advisor = navigationContext.Parameters["Advisor"] as Advisor;
            CanRemoveAdvisor = false;
            if (Advisor != null && Advisor.Id >0)
            {
                bool hasInvestors = AdvisorAccess.AdvisorHasInvestor(Advisor.Id);
                if (!hasInvestors) CanRemoveAdvisor = true;
            }
            if (Advisor != null) Advisor.PropertyChanged -= Advisor_PropertyChanged;
            RaisePropertyChanged("Advisor");
            copyOfAdvisor = new Advisor(Advisor);

            TabTitle = Advisor.FullName;
            if (string.IsNullOrEmpty(TabTitle)) TabTitle = "neuer Berater";

            var task = GetAllAdvisorAsync();
            Advisor.PropertyChanged += Advisor_PropertyChanged;
        }

        private void Advisor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName=="Title" || e.PropertyName=="FirstName" || e.PropertyName=="LastName")
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (!string.IsNullOrEmpty(Advisor.Title)) stringBuilder.Append(Advisor.Title + " ");
                if (!string.IsNullOrEmpty(Advisor.FirstName)) stringBuilder.Append(Advisor.FirstName + " ");
                if (!string.IsNullOrEmpty(Advisor.LastName)) stringBuilder.Append(Advisor.LastName );
                Advisor.FullName = stringBuilder.ToString();
            }
        }

        private async Task GetAllAdvisorAsync()
        {
            ReportsToList =new List<Advisor>(await AdvisorAccess.GetAllHqtAdvisorsAsync());
            if (Advisor.Id > 0)
            {
                for (int i= 0; i < ReportsToList.Count; i++)
                {
                    Advisor item = ReportsToList.ElementAt(i);
                    if (item.Id != Advisor.Id) continue;
                    ReportsToList.RemoveAt(i);
                    break;
                }
            }
            RaisePropertyChanged("ReportsToList");
        }
    }
}
