using BLL_DataAccess;
using BLL_DataModels;
using BLL_Private_Equity.Berechnungen;
using BLL_Private_Equity.Events;
using BLL_Infrastructure;
using BLL_Prism;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace BLL_Private_Equity.Views
{
    public class ShowPeFundCommitmentsViewModel: HqtBindableBase, IRegionManagerAware
    {

     
        private InvestorCommitment selectedInvestorCommitment = null;
        
      

        private ObservableCollection<PeFundHirarchy> hirarchies;

        public ObservableCollection<PeFundHirarchy> Hirarchies
        {
            get { return this.hirarchies; }
            set { SetProperty(ref hirarchies, value); }
        }

        private bool canShowInvestor =false;

        public bool CanShowInvestor
        {
            get { return this.canShowInvestor; }
            set { SetProperty(ref canShowInvestor, value); }
        }

        private PeFund fund;

        public PeFund Fund
        {
            get { return this.fund; }
            set { SetProperty(ref fund, value); }
        }
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;

        public IRegionManager RegionManager { get; set; }
        public ICommand ShowInvestorCommand { get; set; }
        public ICommand ShowInvestorCommitmentsCommand { get; set; }
        public ICommand StartCashFlowCommand { get; set; }
        public ICommand InvestorRowSelectedCommand { get; set; }
        public ICommand OnWindowLoadedCommand { get; set; }

        public ShowPeFundCommitmentsViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            ShowInvestorCommand = new DelegateCommand(OnShowInvestor).ObservesCanExecute(() => CanShowInvestor);
            ShowInvestorCommitmentsCommand = new DelegateCommand(OnShowInvestorCommitment).ObservesCanExecute(() => CanShowInvestor);
            StartCashFlowCommand = new DelegateCommand(OnStartCashFlow);
            InvestorRowSelectedCommand = new DelegateCommand<object>(OnInvestorRowSelected);
            OnWindowLoadedCommand = new DelegateCommand(OnWindowLoaded);
        }

        private void OnWindowLoaded()
        {
           
            eventAggregator.GetEvent<ShowPeFundDiagramEvent>().Publish(new ShowPeFundDiagram()
            {
                hirarchy = Hirarchies,
                fund = Fund
            });
        }
        public static T FindChild<T>(DependencyObject parent)
     where T : DependencyObject
        {
            // confirm parent is valid.
            if (parent == null) return null;
            if (parent is T) return parent as T;

            DependencyObject foundChild = null;

            if (parent is FrameworkElement) (parent as FrameworkElement).ApplyTemplate();

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                foundChild = FindChild<T>(child);
                if (foundChild != null) break;
            }

            return foundChild as T;
        }

        private void OnInvestorRowSelected(object obj)
        {
            if (obj != null)
            {
                selectedInvestorCommitment = obj as InvestorCommitment;
                CanShowInvestor = true;
            }
            else
            {
                selectedInvestorCommitment = null;
                CanShowInvestor = false;
            }
        }

        private void OnStartCashFlow()
        {
            throw new NotImplementedException();
        }

        private void OnShowInvestorCommitment()
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("Investor", selectedInvestorCommitment.Investor);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ShowInvestorCommitments, parameters);
        }

        private void OnShowInvestor()
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("Investor", selectedInvestorCommitment.Investor);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.InvestorDetails, parameters);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Fund = navigationContext.Parameters["Fund"] as PeFund;
            // set tabtitle
            if (string.IsNullOrEmpty(fund.FundHqTrustNumber))
            {
                TabTitle = fund.FundHqTrustNumber + " (Commitmnets)";
            }
            else
            {
                TabTitle = fund.FundShortName + " (Commitments)";
            }
          

            Hirarchies = new ObservableCollection<PeFundHirarchy>();
            Hirarchies.Add(new PeFundHirarchy(fund));

     
        }
        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            if (Fund == null) return true;
            PeFund f= navigationContext.Parameters["Fund"] as PeFund;

            if (f.Id == Fund.Id) return true;
            return false;

        }

    }
}
