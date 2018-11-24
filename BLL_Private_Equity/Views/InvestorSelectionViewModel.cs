using BLL_DataModels;
using BLL_DataAccess;
using BLL_Prism;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Windows;
using Prism.Commands;
using BLL_Infrastructure;
using Prism.Events;
using BLL_Private_Equity.Events;
using System.Linq;

namespace BLL_Private_Equity.Views
{
    public class InvestorSelectionViewModel: HqtBindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        InvestorAccess investorAccess = new InvestorAccess();

        public DelegateCommand LoadedCommand { get; set; }
        public DelegateCommand DeleteFilterCommand { get; set; }
        public DelegateCommand FilterCommand { get; set; }
        public DelegateCommand InvestorSelectedCommand { get; set; }

        private ObservableCollection<Investor> investors;

        private ICollectionView investorList;

        public ICollectionView InvestorList
        {
            get { return this.investorList; }
            set { SetProperty(ref investorList, value); }
        }


        private Investor selectedInvestor;

        public Investor SelectedInvestor
        {
            get { return this.selectedInvestor; }
            set { SetProperty(ref selectedInvestor, value); }
        }

        private bool investorsLoading = true;

        public bool InvestorsLoading
        {
            get { return this.investorsLoading; }
            set { SetProperty(ref investorsLoading, value); }
        }

        private string filterText;

        public string FilterText
        {
            get { return this.filterText; }
            set { SetProperty(ref filterText, value); }
        }


        public InvestorSelectionViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            LoadedCommand = new DelegateCommand(OnLoaded);
            DeleteFilterCommand = new DelegateCommand(OnDeleteFilter);
            FilterCommand = new DelegateCommand(OnFilter);
            InvestorSelectedCommand = new DelegateCommand(OnInvestorSelected);
            eventAggregator.GetEvent<InvestorCollectionActionEvent>().Subscribe(OnInvestorCollectionAction);
        }

        private void OnInvestorSelected()
        {
            if (SelectedInvestor == null) return;
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Investor", SelectedInvestor);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.InvestorDetails, parameter);
        }

        private void OnFilter()
        {
            if (InvestorList == null) return;
            if (InvestorList.Filter==null)
            {
                InvestorList.Filter = FilterInvestor;
                return;
            }

            InvestorList.Refresh();
        }

        private bool FilterInvestor(object obj)
        {
            Investor investor = obj as Investor;
            if (investor.InvestorHqTrustAccount!= null && investor.InvestorHqTrustAccount.StartsWith(FilterText)) return true;
            if (investor.InvestorReference != null && investor.InvestorReference.ToLower().Contains(FilterText.ToLower())) return true;
            if (investor.IName.FullName != null && investor.IName.FullName.ToLower().Contains(FilterText.ToLower())) return true;
            return false;
        }

        private void OnDeleteFilter()
        {
            // Remove Filter from InvestorList
            InvestorList.Filter = null;
            FilterText = string.Empty;
        }

        private void OnLoaded()
        {            
            var task = LoadInvestorsAsync();
            
        }

        private async void OnInvestorCollectionAction(InvestorCollectionAction obj)
        {
            if (obj.action == CollectionAction.removed)
            {
                Investor investor = investors.FirstOrDefault(i => i.Id == obj.investor.Id);
                investors.Remove(investor);
            }
            else if (obj.action == CollectionAction.added)
            {
                investors.Add(obj.investor);
            }
            else if (obj.action == CollectionAction.updated)
            {
                Investor investor = investors.FirstOrDefault(i => i.Id == obj.investor.Id);
                investor.InvestorHqTrustAccount = obj.investor.InvestorHqTrustAccount;
                investor.InvestorReference = obj.investor.InvestorReference;
                investor.IName.FullName = obj.investor.IName.FullName;
            }
            else if (obj.action == CollectionAction.reload)
            {
                await LoadInvestorsAsync();
            }
        }

        private async Task  LoadInvestorsAsync()
        {
            InvestorsLoading = true;
            investors = new ObservableCollection<Investor>( await investorAccess.GetAllInvestorsAsync());
            

            InvestorList = CollectionViewSource.GetDefaultView(investors);
            
            

            SortDescription sortDescription = new SortDescription()
            {
                Direction = ListSortDirection.Ascending,
                PropertyName = "InvestorReference"
            };

            InvestorList.SortDescriptions.Add(sortDescription);
            InvestorsLoading = false;

        }
    }
}
