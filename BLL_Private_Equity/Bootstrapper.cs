using BLL_Infrastructure;
using BLL_Prism;
using BLL_Private_Equity.Views;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using System.Windows;

namespace BLL_Private_Equity
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return new Views.Shell();
        }
        protected override void InitializeShell()
        {
            base.InitializeShell();
            Application.Current.MainWindow.Show();
        }
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            ViewModelLocationProvider.SetDefaultViewModelFactory((type) => Container.Resolve(type));

            Container.RegisterType<IRegionNavigationContentLoader, ScopedRegionNavigationContentLoader>(new ContainerControlledLifetimeManager());
            Container.RegisterType<object, StatusBarView>(ViewNames.StatusBarView);
            //Container.RegisterType<object, ApplicationSettings>(ViewNames.ApplicationSettings);
            Container.RegisterType<object, InvestorSelection>(ViewNames.InvestorSelection);
            Container.RegisterType<object, PeFundSelection>(ViewNames.PeFundSelection);
            Container.RegisterType<object, PeFundDetail>(ViewNames.PeFundDetail);
            Container.RegisterType<object, InvestorDetails>(ViewNames.InvestorDetails);
            Container.RegisterType<object, ShowInvestorList>(ViewNames.ShowInvestorList);
            Container.RegisterType<object, ShowInvestorCommitments>(ViewNames.ShowInvestorCommitments);
            Container.RegisterType<object, ShowInvestorPcap>(ViewNames.ShowInvestorPcap);
            Container.RegisterType<object, EditInvestorCommitment>(ViewNames.EditInvestorCommitment);
            Container.RegisterType<object, ShowPeFundCommitments>(ViewNames.ShowPeFundCommitments);
            Container.RegisterType<object, PeFundDiagram>(ViewNames.PeFundDiagram);
            Container.RegisterType<object, ExportInvestorList>(ViewNames.ExportInvestorList);
            Container.RegisterType<object, ControlCashFlow>(ViewNames.ControlCashFlow);
            Container.RegisterType<object, CheckInvestorData>(ViewNames.CheckInvestorData);
            Container.RegisterType<object, EditCashFlowData>(ViewNames.EditCashFlowData);
            Container.RegisterType<object, EditInvestorSplit>(ViewNames.EditInvestorSplit);
            Container.RegisterType<object, CheckLetters>(ViewNames.CheckLetters);
            Container.RegisterType<object, PeFundCashFlows>(ViewNames.PeFundCashFlows);
            Container.RegisterType<object, InitiatorSelection>(ViewNames.InitiatorSelection);
            Container.RegisterType<object, InitiatorDetail>(ViewNames.InitiatorDetail);
            //Container.RegisterType<object, HqtAdvisorSelection>(ViewNames.HqtAdvisorSelection);
            //Container.RegisterType<object, HqtAdvisorDetail>(ViewNames.HqtAdvisorDetail);
            Container.RegisterType<object, ClientAdvisorSelection>(ViewNames.ClientAdvisorSelection);
            Container.RegisterType<object, ClientAdvisorDetail>(ViewNames.ClientAdvisorDetail);
            Container.RegisterType<object, ClientAdvisorInvestors>(ViewNames.ClientAdvisorInvestors);
            //Container.RegisterType<object, ImportPsPlus>(ViewNames.ImportPsPlus);
            //Container.RegisterType<object, ShowPsPlusCashFlows>(ViewNames.ShowPsPlusCashFlows);
            Container.RegisterType<object, EditPcaps>(ViewNames.EditPcaps);
            Container.RegisterType<object, ShowInvestorCashFlows>(ViewNames.ShowInvestorCashflows);
            //Container.RegisterType<object, AnalyzeSettings>(ViewNames.AnalyzeSettings);
            //Container.RegisterType<object, AnalyzeContainer>(ViewNames.AnalyzeContainer);
            //Container.RegisterType<object, JCurveChart>(ViewNames.JCurveChart);
        }
        protected override void ConfigureModuleCatalog()
        {
            var catalog = (ModuleCatalog)ModuleCatalog;
            if (catalog != null)
                catalog.AddModule(typeof(BLL_Settings.BLLSettingsModule));
        }
        protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            IRegionBehaviorFactory behaviors = base.ConfigureDefaultRegionBehaviors();
            behaviors.AddIfMissing(RegionManagerAwareBehavior.BehaviorKey, typeof(RegionManagerAwareBehavior));
            behaviors.AddIfMissing(RibbonRegionBehavior.BehaviorKey, typeof(RibbonRegionBehavior));
            return behaviors;
        }
    }
}
