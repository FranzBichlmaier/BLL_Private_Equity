using BLL_Prism;
using Prism.Regions;
using System;
using BLL_DataModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Documents.Spreadsheet.Model;
using BLL_Private_Equity.Berechnungen;
using BLL_DataAccess;
using System.Collections.ObjectModel;
using Prism.Events;
using BLL_Private_Equity.Events;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using BLL_Infrastructure;

namespace BLL_Private_Equity.Views
{
    public class ImportNavViewModel: HqtBindableBase
    {
        ExcelHelperFunctions sheetFunctions;
        Workbook workbook = null;
        List<ExcelNav> navList = new List<ExcelNav>();
        InvestorAccess investorAccess = new InvestorAccess();
        string currentFundWkn = string.Empty;
        string currentInvestorNumber = string.Empty;
        ImportCommitment commitment = null;
        SubscriptionToken token; 
        public ICommand CloseTabCommand { get; set; }


        private ObservableCollection<string> errorMessages = new ObservableCollection<string>();

        public ObservableCollection<string> ErrorMessages

        {
            get { return this.errorMessages; }
            set { SetProperty(ref errorMessages, value); }
        }
        private ObservableCollection<ImportCommitment> iCommitments;

        private string numberOfRecords;

        public string NumberOfRecords
        {
            get { return this.numberOfRecords; }
            set { SetProperty(ref numberOfRecords, value); }
        }

        private string currentFundName;

        public string CurrentFundName
        {
            get { return this.currentFundName; }
            set { SetProperty(ref currentFundName, value); }
        }

        private string currentRecordNumber;

        public string CurrentRecordNumber
        {
            get { return this.currentRecordNumber; }
            set { SetProperty(ref currentRecordNumber, value); }
        }

        private int totalRecords;
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;

        public int TotalRecords
        {
            get { return this.totalRecords; }
            set
            {
                SetProperty(ref totalRecords, value);
                NumberOfRecords = $"Es sind {TotalRecords:n0} Zeilen in der ExcelDatei vorhanden."; 
            }
        }

        private bool readExcel = false;

        public bool ReadExcel
        {
            get { return this.readExcel; }
            set { SetProperty(ref readExcel, value); }
        }
        public ImportNavViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            eventAggregator.GetEvent<ImportEvent>().Subscribe(OnImportEvent);
            CloseTabCommand = new DelegateCommand(OnCloseTab);
        }

        private void OnCloseTab()
        {
            var activeView = regionManager.Regions[RegionNames.TabControlRegion].ActiveViews.ElementAt(0);
            if (activeView == null) return;
            regionManager.Regions[RegionNames.TabControlRegion].Remove(activeView);
        }

        private void OnImportEvent(string obj)
        {
            Application.Current.Dispatcher.BeginInvoke(
                   DispatcherPriority.Normal,
                   new Action(() => ErrorMessages.Add(obj)));
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            workbook = navigationContext.Parameters["Workbook"] as Workbook;
            iCommitments = navigationContext.Parameters["Commitments"] as ObservableCollection<ImportCommitment>;
            ReadExcel = true;
            foreach(ImportCommitment i in iCommitments)
            {
                if (string.IsNullOrEmpty(i.InvestorNumber) || string.IsNullOrEmpty(i.PeFundNumber))
                {

                }
            }
            TabTitle = "NAV - Import";
            try
            {
                await Task.Run (() =>CreateNavListAsync());

                await Task.Run(() => ImportNavListAsync());
            }
            catch (Exception ex)
            {               
            }
            ReadExcel = false;
          
        }

        private async Task ImportNavListAsync()
        {
            for (int counter = 0; counter < navList.Count; counter++) 
            {
                if (counter%100 ==0)
                {
                    CurrentRecordNumber = $"Es wurden {counter:n0} von {navList.Count:n0} Sätzen verarbeitet.";
                }
                ExcelNav nav = navList.ElementAt(counter);

                InvestorPcap pcap = investorAccess.GetPcapForInvestorByCommitmentAndDate(nav.InvestorCommitmentId, nav.NavDate);
                if (pcap == null)
                {
                    // insert new pcap
                    InvestorPcap newPcap = new InvestorPcap()
                    {
                        AsOfDate = nav.NavDate,
                        FinalPcapAmount = nav.NavAmount,
                        InvestorCommitmentId = nav.InvestorCommitmentId,
                        DateOfFinalPcap = nav.NavDate,
                        EstimatedPcapAmount = nav.NavAmount
                    };
                    try
                    {
                        investorAccess.InsertOrUpdateInvestorPcap(newPcap);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessages.Add(ex.Message);
                    }    
                }
                else
                {
                    if (pcap.FinalPcapAmount == nav.NavAmount) continue;
                    pcap.FinalPcapAmount = nav.NavAmount;
                    pcap.DateOfFinalPcap = nav.NavDate;
                    pcap.EstimatedPcapAmount = nav.NavAmount;
                    try
                    {
                        investorAccess.InsertOrUpdateInvestorPcap(pcap);
                    }
                    catch (Exception ex)
                    {
                        ErrorMessages.Add(ex.Message);
                    }
                }
            }
            CurrentRecordNumber = $"Es wurden alle Datensätze verarbeitet.";
        }
        /// <summary>
        /// GetNavRow reads the excel sheet and returns ExcelNav. 
        /// It returns null if either fund, investor or commitment is missing
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private ExcelNav GetNavRow(int row)
        {
            ExcelNav nav = new ExcelNav();
            try
            {
                nav.NavAmount = sheetFunctions.GetAmount(row, 3);
            }
            catch (Exception)
            {
                nav.NavAmount = 0;
            }
            nav.NavDate = sheetFunctions.GetDate(row, 2);
            nav.InvestorNumber = sheetFunctions.GetText(row, 1);
            nav.FundWkn = sheetFunctions.GetText(row, 0);

            if (commitment == null || currentInvestorNumber != nav.InvestorNumber ||currentFundWkn != nav.FundWkn)
            {
                currentInvestorNumber = nav.InvestorNumber;
                currentFundWkn = nav.FundWkn;
                commitment = iCommitments.FirstOrDefault(c => c.InvestorNumber.Equals(nav.InvestorNumber) && c.PeFundNumber.Equals(nav.FundWkn));
                if (commitment == null)
                {
                    eventAggregator.GetEvent<ImportEvent>()
                        .Publish($"Ein Commitment für den Investor {currentInvestorNumber} und die Beteiligungsnummer {currentFundWkn} wurde nicht gefunden.");                   
                    return null;
                }
            }

            nav.InvestorId = commitment.InvestorId;
            nav.PeFundId = commitment.PeFundId;
            nav.InvestorCommitmentId = commitment.InvestorCommitmentId;

            return nav;
        }

        private async Task CreateNavListAsync()
        {
            await Task.Run(() =>
            {
                foreach (Worksheet ws in workbook.Sheets)
                {
                    if (!ws.Name.Equals("NAV")) continue;

                    sheetFunctions = new ExcelHelperFunctions(ws);
                    TotalRecords = sheetFunctions.RowCount;

                    // reading starts with second row as first row contains headlines

                    for (int row = 1; row < sheetFunctions.RowCount; row++)
                    {
                        ExcelNav nav = GetNavRow(row);
                        if (nav != null) navList.Add(nav);
                        else
                        {

                        }
                    }
                }
            });

        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
    }
}
