using BLL_Private_Equity.Berechnungen;
using BLL_Prism;
using BLL_DataModels;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DateTimeFunctions;
using BLL_Private_Equity.Comparer;
using BLL_Infrastructure;
using Prism.Events;
using BLL_Private_Equity.Events;
using System.Windows.Input;
using Prism.Commands;

namespace BLL_Private_Equity.Views
{
    public class AnalyzeContainerViewModel: HqtBindableBase
    {

        private string headerText;

        public string HeaderText
        {
            get { return this.headerText; }
            set { SetProperty(ref headerText, value); }
        }
        private AnalyzingParameter analyzingParameter = null;
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        DateDifferences timeFunctions = new DateDifferences();
        List<ExtendedPcap> pcaps = new List<ExtendedPcap>();
        PeFundResults PeFundResults;
        bool hasBeenLoaded = false;

        public ICommand LoadedCommand { get; set; }

        public AnalyzeContainerViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;

            LoadedCommand = new DelegateCommand(OnLoaded);
        }

        private void OnLoaded()
        {
            hasBeenLoaded = true;
            if (AnalyzingParameter == null) return;
            CreateHeaderText();
            CreateDataForCharts();
        }

        public AnalyzingParameter AnalyzingParameter
        {
            get { return this.analyzingParameter; }
            set { SetProperty(ref analyzingParameter, value); }
        }

       

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            AnalyzingParameter = navigationContext.Parameters["Parameter"] as AnalyzingParameter;
            TabTitle = "Portfolio Analyse";

            if (hasBeenLoaded == false) return;
            CreateHeaderText();
            CreateDataForCharts();
        }

        private void CreateHeaderText()
        {
            StringBuilder builder = new StringBuilder($"Analyse des Zeitraumes {AnalyzingParameter.StartDate:d} bis {AnalyzingParameter.EndDate:d}. (");
            foreach (ExtendedCommitment commitment in AnalyzingParameter.Commitments)
            {
                builder.Append(commitment.Commitment.PeFund.FundName + ", ");
            }
            HeaderText = builder.ToString();
            HeaderText = HeaderText.Substring(0,HeaderText.Length-2);
            HeaderText = HeaderText + ")";
        }

        private void CreateDataForCharts()
        {
            pcaps = new List<ExtendedPcap>();
            if (AnalyzingParameter.Period == AnalyzingParameterPeriod.Auto)
            {
                int quarters = timeFunctions.Quarters(AnalyzingParameter.StartDate, AnalyzingParameter.EndDate);

                // bis 5 Jahre => Quartal
                // bis 10 Jahre => Halbjahr
                // bis 20 Jahre => Jahr
                // bis 40 Jahre => 2 Jahre
                // >40 Jahre => 5 Jahre

                if (quarters <= 20) AnalyzingParameter.Period = AnalyzingParameterPeriod.Quarter;
                else if (quarters <= 40) AnalyzingParameter.Period = AnalyzingParameterPeriod.SixMonth;
                else if (quarters <= 80) AnalyzingParameter.Period = AnalyzingParameterPeriod.Year;
                else if (quarters <= 160) AnalyzingParameter.Period = AnalyzingParameterPeriod.TwoYear;
                else AnalyzingParameter.Period = AnalyzingParameterPeriod.FiveYear;
            }

            switch (AnalyzingParameter.Period)
            {
                case AnalyzingParameterPeriod.Quarter:
                    {
                        DateTime currentDate = AnalyzingParameter.StartDate;
                        do
                        {
                            ExtendedPcap newPcap = new ExtendedPcap()
                            {
                                AsOfDate = currentDate
                            };
                            pcaps.Add(newPcap);
                            currentDate = timeFunctions.NextQuarter(currentDate);

                        } while (currentDate <= AnalyzingParameter.EndDate);
                        break;
                    }
                case AnalyzingParameterPeriod.SixMonth:
                    {
                        DateTime currentDate = AnalyzingParameter.StartDate;
                        if (currentDate.Month != 6 && currentDate.Month != 12) currentDate = timeFunctions.NextHalfYear(currentDate);
                        ExtendedPcap newPcap = new ExtendedPcap()
                        {
                            AsOfDate = currentDate
                        };
                        pcaps.Add(newPcap);
                        do
                        {
                            currentDate = timeFunctions.NextHalfYear(currentDate);
                            newPcap = new ExtendedPcap()
                            {
                                AsOfDate = currentDate
                            };
                            pcaps.Add(newPcap);
                            

                        } while (currentDate <= AnalyzingParameter.EndDate);
                        pcaps.ElementAt(pcaps.Count - 1).AsOfDate = AnalyzingParameter.EndDate;
                        break;
                    }
                case AnalyzingParameterPeriod.Year:
                    {
                        DateTime currentDate = AnalyzingParameter.StartDate;
                        if (currentDate.Month != 12) currentDate = timeFunctions.NextYearEnd(currentDate);
                        ExtendedPcap newPcap = new ExtendedPcap()
                        {
                            AsOfDate = currentDate
                        };
                        pcaps.Add(newPcap);
                        do
                        {
                            currentDate = currentDate.AddYears(1);
                            newPcap = new ExtendedPcap()
                            {
                                AsOfDate = currentDate
                            };
                            pcaps.Add(newPcap);
                            

                        } while (currentDate <= AnalyzingParameter.EndDate);
                        pcaps.ElementAt(pcaps.Count - 1).AsOfDate = AnalyzingParameter.EndDate;
                        break;
                    }
                case AnalyzingParameterPeriod.TwoYear:
                    {
                        DateTime currentDate = AnalyzingParameter.StartDate;
                        if (currentDate.Month != 12) currentDate = timeFunctions.NextYearEnd(currentDate);
                        ExtendedPcap newPcap = new ExtendedPcap()
                        {
                            AsOfDate = currentDate
                        };
                        pcaps.Add(newPcap);
                        do
                        {
                            currentDate = currentDate.AddYears(2);
                            newPcap = new ExtendedPcap()
                            {
                                AsOfDate = currentDate
                            };
                            pcaps.Add(newPcap);
                            

                        } while (currentDate <= AnalyzingParameter.EndDate);
                        pcaps.ElementAt(pcaps.Count - 1).AsOfDate = AnalyzingParameter.EndDate;
                        break;
                    }
                case AnalyzingParameterPeriod.FiveYear:
                    {
                        DateTime currentDate = AnalyzingParameter.StartDate;
                        if (currentDate.Month != 12) currentDate = timeFunctions.NextYearEnd(currentDate);
                        ExtendedPcap newPcap = new ExtendedPcap()
                        {
                            AsOfDate = currentDate
                        };
                        pcaps.Add(newPcap);
                        do
                        {
                            currentDate = currentDate.AddYears(5);
                            newPcap = new ExtendedPcap()
                            {
                                AsOfDate = currentDate
                            };
                            pcaps.Add(newPcap);                            

                        } while (currentDate <= AnalyzingParameter.EndDate);
                        pcaps.ElementAt(pcaps.Count - 1).AsOfDate = AnalyzingParameter.EndDate;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            // fill pcaps with amounts using cashflows and NAVs of the commitments
            CashflowComparer cashflowComparer = new CashflowComparer();
            PcapComparer pcapComparer = new PcapComparer();
            foreach (ExtendedCommitment commitment in AnalyzingParameter.Commitments)
            {
                // sort cashflows by effectivedate ascending and pcaps by asofDate
               
                commitment.Commitment.InvestorCashFlows.Sort(cashflowComparer);
                commitment.Commitment.InvestorPcaps.Sort(pcapComparer);

                foreach(InvestorCashFlow cashflow in commitment.Commitment.InvestorCashFlows)
                {
                    ExtendedPcap pcap1 = FindPcap(cashflow.EffectiveDate);
                    // pcap can be null if cashflow date is after endDate
                    if (pcap1 == null) continue;
                    if (cashflow.CashFlowType=="Capital Call")
                    {
                        pcap1.CallsInPeriod += cashflow.CashFlowAmount;
                    }
                    if (cashflow.CashFlowType =="Distribution")
                    {
                        pcap1.DistributionsInPeriod += (cashflow.CashFlowAmount + cashflow.RecallableAmount);
                        pcap1.CallsInPeriod -= cashflow.RecallableAmount;
                    }
                }
                // add NAVs
                foreach(InvestorPcap nav in commitment.Commitment.InvestorPcaps)
                {

                    ExtendedPcap pcap2 = pcaps.FirstOrDefault(p => p.AsOfDate.Date == nav.AsOfDate.Date);
                    // pcap is null depending on period
                    if (pcap2 == null) continue;
                    pcap2.NavAmount += nav.FinalPcapAmount;
                }
            }

            // calculate cumulated cashflows

            ExtendedPcap pcap = pcaps.ElementAt(0);
            double totalCalls = 0;
            double totalDistributions = 0;
            foreach(ExtendedPcap p in pcaps)
            {
                p.CallsSinceInception = totalCalls+p.CallsInPeriod;
                p.DistributionsSinceInception = totalDistributions+p.DistributionsInPeriod;
                totalCalls = p.CallsSinceInception;
                totalDistributions = p.DistributionsSinceInception;
            }

            // calculate results for portfolio
            List<InvestorCommitment> commitmentList = new List<InvestorCommitment>();
            foreach(ExtendedCommitment c in AnalyzingParameter.Commitments)
            {
                commitmentList.Add(c.Commitment);
            }
            PeFundResults = new PeFundResults(commitmentList, AnalyzingParameter.StartDate, AnalyzingParameter.EndDate);

            // Create Dictionaries for PieCharts
            // Fire an Event of type AnalyzePortfolioEvent; Views listen to this event

            AnalyzePortfolio analyzePortfolio = new AnalyzePortfolio()
            {
                AnalyzingParameter = AnalyzingParameter,
                Pcaps = pcaps,
                PeFundResults = PeFundResults
            };

            eventAggregator.GetEvent<AnalyzePortfolioEvent>().Publish(analyzePortfolio);
           
        }

        private ExtendedPcap FindPcap(DateTime effectiveDate)
        {
            foreach(ExtendedPcap pcap in pcaps)
            {
                if (effectiveDate > pcap.AsOfDate) continue;
                return pcap;
            }
            return null;
        }
    }
}
