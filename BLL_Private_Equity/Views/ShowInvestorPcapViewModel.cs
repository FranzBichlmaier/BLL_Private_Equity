using DateTimeFunctions;
using BLL_DataAccess;
using BLL_DataModels;
using BLL_Private_Equity.Berechnungen;
using BLL_Private_Equity.Comparer;
using BLL_Infrastructure;
using BLL_Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace BLL_Private_Equity.Views
{
    public class ShowInvestorPcapViewModel: HqtBindableBase, IRegionManagerAware
    {
        DateDifferences dateFunctions = new DateDifferences();
        InvestorAccess investorAccess = new InvestorAccess();

        private ObservableCollection<ExtendedPcap> pcaps = new ObservableCollection<ExtendedPcap>();

        public ICollectionView Pcaps { get; set; }
        public ICommand RowEditEndedCommand { get; set; }
        public InteractionRequest<INotification> NotificationRequest { get; set; }

        private InvestorCommitment investorCommitment=null;
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;

        public InvestorCommitment InvestorCommitment
        {
            get { return this.investorCommitment; }
            set { SetProperty(ref investorCommitment, value); }
        }

        private string headerText = string.Empty;

        public string HeaderText
        {
            get { return this.headerText; }
            set { SetProperty(ref headerText, value); }
        }

        public IRegionManager RegionManager { get; set; }

        public ShowInvestorPcapViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            RowEditEndedCommand = new DelegateCommand(OnRowEditEnded);

            NotificationRequest = new InteractionRequest<INotification>();
        }

        private void OnRowEditEnded()
        {
            if (Pcaps.CurrentItem == null) return;
            ExtendedPcap epcap = Pcaps.CurrentItem as ExtendedPcap;

            InvestorPcap pcap = InvestorCommitment.InvestorPcaps.FirstOrDefault(p => p.AsOfDate == epcap.AsOfDate);
            if (pcap == null) return;
            pcap.FinalPcapAmount = epcap.NavAmount;
            try
            {
                investorAccess.InsertOrUpdateInvestorPcap(pcap);
            }
            catch (Exception ex)
            {
                NotificationRequest.Raise(new Notification()
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = ex.Message
                });
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            InvestorCommitment = navigationContext.Parameters["Commitment"] as InvestorCommitment;
            InvestorCommitment.Investor = investorAccess.GetInvestorById((int)InvestorCommitment.InvestorId);
            HeaderText = $"Pcaps / NAV für {InvestorCommitment.Investor.IName.FullName} der Beteiligung an {InvestorCommitment.PeFund.FundName}.";
            TabTitle = $"{InvestorCommitment.Investor.InvestorHqTrustAccount} (Pcap: {InvestorCommitment.PeFund.FundName})";
            if (InvestorCommitment.InvestorCashFlows.Count ==0)
            {
                HeaderText = headerText + " Es sind keine Pcaps vorhanden";
                return;
            }
            pcaps.Clear();
            FillPcaps();
            Pcaps = CollectionViewSource.GetDefaultView(pcaps);
            RaisePropertyChanged("Pcaps");
        }

        private void FillPcaps()
        {
            DateTime firstCashFlowDate = InvestorCommitment.InvestorCashFlows.Min(p => p.EffectiveDate);
            firstCashFlowDate = dateFunctions.NextQuarter(firstCashFlowDate);
            DateTime maxCashFlowDate = dateFunctions.PreviousQuarter(DateTime.Now);

            // fill Pcaps

            DateTime currentDate = firstCashFlowDate;
            do
            {
                ExtendedPcap newPcap = new ExtendedPcap()
                {
                    AsOfDate = currentDate
                };
                pcaps.Add(newPcap);
                currentDate = dateFunctions.NextQuarter(currentDate);

            } while (currentDate <= maxCashFlowDate);

            CashflowComparer cashflowComparer = new CashflowComparer();
            PcapComparer pcapComparer = new PcapComparer();

            InvestorCommitment.InvestorCashFlows.Sort(cashflowComparer);
            InvestorCommitment.InvestorPcaps.Sort(pcapComparer);

            foreach (InvestorCashFlow cashflow in InvestorCommitment.InvestorCashFlows)
            {
                ExtendedPcap pcap1 = FindPcap(cashflow.EffectiveDate);
                // pcap can be null if cashflow date is after endDate
                if (pcap1 == null) continue;
                if (cashflow.CashFlowType == "Capital Call")
                {
                    pcap1.CallsInPeriod += cashflow.CashFlowAmount;
                }
                if (cashflow.CashFlowType == "Distribution")
                {
                    pcap1.DistributionsInPeriod += (cashflow.CashFlowAmount + cashflow.RecallableAmount);
                    pcap1.CallsInPeriod -= cashflow.RecallableAmount;
                }
            }
            // add NAVs
            foreach (InvestorPcap nav in InvestorCommitment.InvestorPcaps)
            {
                ExtendedPcap pcap2 = FindPcap(nav.AsOfDate);
                // pcap can be null if cashflow date is after endDate
                if (pcap2 == null) continue;
                pcap2.NavAmount += nav.FinalPcapAmount;
            }


            // calculate cumulated cashflows and profit / Loss for the quarter
            ExtendedPcap previousPcap = new ExtendedPcap();
            ExtendedPcap pcap = pcaps.ElementAt(0);
            double totalCalls = 0;
            double totalDistributions = 0;
            foreach(ExtendedPcap p in pcaps)
            {
                p.CallsSinceInception = totalCalls+p.CallsInPeriod;
                p.DistributionsSinceInception = totalDistributions+p.DistributionsInPeriod;
                totalCalls = p.CallsSinceInception;
                totalDistributions = p.DistributionsSinceInception;
                p.ProfitLossInQuarter = p.NavAmount + p.CallsInPeriod + p.DistributionsInPeriod - previousPcap.NavAmount;
                previousPcap = p;
            }          
            
        }
        private ExtendedPcap FindPcap(DateTime effectiveDate)
        {
            foreach (ExtendedPcap pcap in pcaps)
            {
                if (effectiveDate > pcap.AsOfDate) continue;
                return pcap;
            }
            return null;
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            InvestorCommitment newInvestorCommitment = navigationContext.Parameters["Commitment"] as InvestorCommitment;

            if (InvestorCommitment == null) return true;
            if (newInvestorCommitment.Id == InvestorCommitment.Id) return true;
            return false;
        }
    }
}
