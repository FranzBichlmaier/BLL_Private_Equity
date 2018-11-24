using BLL_DataModels;
using BLL_Private_Equity.Events;
using BLL_Private_Equity.RibbonTabs;
using BLL_Infrastructure;
using BLL_Prism;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;


namespace BLL_Private_Equity.Views
{
    public class ShellViewModel : HqtBindableBase
    {
        
        private IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;

        public DelegateCommand WindowLoadedCommand { get; set; }
        public DelegateCommand ExitCommand { get; set; }
        public DelegateCommand<string> SettingsCommand { get; set; }
        public DelegateCommand AddInvestorCommand { get; set; }
        public DelegateCommand AddPeFundCommand { get; set; }
        public DelegateCommand AddInitiatorCommand { get; set; }
        public DelegateCommand AddAdvisorCommand { get; set; }
        public DelegateCommand AddClientAdvisorCommand { get; set; }
        public DelegateCommand ShowInvestorListCommand { get; set; }
        public DelegateCommand ShowPeFundListCommand {get; set;}
        public DelegateCommand<object> RibbonViewSelectionChangedCommand { get; set; }
        public DelegateCommand StartImportFromPsPlusCommand { get; set; }


        private bool analyzingRequired;

        public bool AnalyzingRequired
        {
            get { return this.analyzingRequired; }
            set { SetProperty(ref analyzingRequired, value); }
        }

        public ShellViewModel( IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            WindowLoadedCommand = new DelegateCommand(OnWindowLoaded);
            ExitCommand = new DelegateCommand(OnExit);
            SettingsCommand = new DelegateCommand<string>(OnSettings);
            AddInvestorCommand = new DelegateCommand(OnAddInvestor);
            AddPeFundCommand = new DelegateCommand(OnAddPeFund);
            AddInitiatorCommand = new DelegateCommand(OnAddInitiator);
            AddAdvisorCommand = new DelegateCommand(OnAddAdvisor);
            AddClientAdvisorCommand = new DelegateCommand(OnAddClientAdvisor);
            ShowInvestorListCommand = new DelegateCommand(OnShowInvestorList);
            ShowPeFundListCommand = new DelegateCommand(OnShowPeFundList);
            RibbonViewSelectionChangedCommand = new DelegateCommand<object>(OnRibbonViewSelectionChanged);
            StartImportFromPsPlusCommand = new DelegateCommand(OnStartImportFromPsPlus);

            Telerik.Windows.Controls.MaterialPalette.Palette.FontSizeS = 10;
            Telerik.Windows.Controls.MaterialPalette.Palette.FontSize = 14;
            Telerik.Windows.Controls.MaterialPalette.Palette.FontSizeL = 16;

            eventAggregator.GetEvent<AnalyzingInformationEvent>().Subscribe(OnAnalyzingRequested);
        }

        private void OnAnalyzingRequested(AnalyzingInformation obj)
        {
            //this property is bound to the isselectedProperty of RadOutlookbarItem Portfolio Analyse

            AnalyzingRequired = true;
        }

        private void OnStartImportFromPsPlus()
        {
            string anwender = Properties.Settings.Default.Anwender;
            switch (anwender)
            {
                case "HQT":
                    {
                        regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ImportPsPlus);
                        break;
                    }
                case "Cavalry":
                    {
                        break;
                    }
            }
            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ImportPsPlus);
        }

        private void OnAddClientAdvisor()
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("ClientAdvisor", new ClientAdvisor());

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ClientAdvisorDetail, parameters);
        }

        private void OnAddAdvisor()
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("Advisor", new Advisor());

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.HqtAdvisorDetail, parameters);
        }

        private void OnAddInitiator()
        {
            NavigationParameters parameters = new NavigationParameters();
            parameters.Add("Initiator", new Initiator());

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.InitiatorDetail, parameters);
        }

        private void OnRibbonViewSelectionChanged(object obj)
        {
            //Telerik.Windows.Controls.RadSelectionChangedEventArgs e = obj as Telerik.Windows.Controls.RadSelectionChangedEventArgs;
        }

        private void OnShowPeFundList()
        {
           
        }

        private void OnShowInvestorList()
        {
            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ShowInvestorList);
        }

        private void OnAddPeFund()
        {
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Fund", new PeFund());

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.PeFundDetail, parameter);
        }

        private void OnAddInvestor()
        {
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Investor", null);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.InvestorDetails, parameter);
        }

        private void OnSettings(string parameter)
        {
            switch (parameter)
            {
                case "Settings":
                    {
                        regionManager.RequestNavigate(RegionNames.AppSettingsRegion, ViewNames.ApplicationSettings);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void OnExit()
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void OnWindowLoaded()
        {
            regionManager.RequestNavigate(RegionNames.StatusBarRegion, ViewNames.StatusBarView);
            //regionManager.RequestNavigate(RegionNames.AppSettingsRegion, ViewNames.ApplicationSettings);
            regionManager.RequestNavigate(RegionNames.InvestorRegion, ViewNames.InvestorSelection);
            regionManager.RequestNavigate(RegionNames.PeFundRegion, ViewNames.PeFundSelection);
            //regionManager.RequestNavigate(RegionNames.InitiatorRegion, ViewNames.InitiatorSelection);           
            //regionManager.RequestNavigate(RegionNames.ClientAdvisorRegion, ViewNames.ClientAdvisorSelection);
            //regionManager.RequestNavigate(RegionNames.AnalyzeSettingsRegion, ViewNames.AnalyzeSettings);

            var tab = Activator.CreateInstance<ApplicationRibbonTab>() as Telerik.Windows.Controls.RadRibbonTab;
             
            regionManager.Regions[RegionNames.RibbonTabRegion].Add(tab);
        }
    }
}
