using BLL_DataAccess;
using BLL_DataModels;
using BLL_Infrastructure;
using BLL_Prism;
using Prism.Commands;
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
    public class ClientAdvisorInvestorsViewModel: HqtBindableBase
    {
        private readonly IRegionManager regionManager;
        private InvestorAccess investorAccess = new InvestorAccess();

        private ObservableCollection<Investor> investorList;

        private Investor selectedInvestor = null;

        public Investor SelectedInvestor
        {
            get { return this.selectedInvestor; }
            set { SetProperty(ref selectedInvestor, value); }
        }

        private ICollectionView investors;

        public ICollectionView Investors
        {
            get { return this.investors; }
            set { SetProperty(ref investors, value); }
        }

        public ICommand ShowInvestorDetailsCommand { get; set; }

        public ClientAdvisorInvestorsViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            ShowInvestorDetailsCommand = new DelegateCommand(OnShowInvestorDetail);
        }

        private void OnShowInvestorDetail()
        {
            if (selectedInvestor == null) return;
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Investor", selectedInvestor);
            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.InvestorDetails, parameter);
        }
        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            int clientAdvisorId = (int)navigationContext.Parameters["ClientAdvisorId"];
            SelectedInvestor = null;

            investorList = new ObservableCollection<Investor>(investorAccess.GetInvestorsByClientAdvisorId(clientAdvisorId));

            if (Investors != null) Investors.CurrentChanged -= Investors_CurrentChanged;

            Investors = CollectionViewSource.GetDefaultView(investorList);
            Investors.CurrentChanged += Investors_CurrentChanged;
            Investors.MoveCurrentToFirst();     // Select first element
            SelectedInvestor = Investors.CurrentItem as Investor;
            TabTitle = SelectedInvestor.ClientAdvisor.AdvisorName.FullName + "(Investors)";

        }

        private void Investors_CurrentChanged(object sender, EventArgs e)
        {
            SelectedInvestor = Investors.CurrentItem as Investor;          
        }
    }
}
