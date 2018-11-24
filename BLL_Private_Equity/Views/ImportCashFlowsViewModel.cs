using BLL_DataAccess;
using BLL_DataModels;
using BLL_Private_Equity.Berechnungen;
using BLL_Private_Equity.Events;
using BLL_Infrastructure;
using BLL_Prism;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;

namespace BLL_Private_Equity.Views
{
    public class ImportCashFlowsViewModel: HqtBindableBase
    {
        FileInfo fileInfo;
        ObservableCollection<ImportCommitment> commitments;
        ObservableCollection<ImportCashFlow> cashFlows;
        InvestorAccess investorAccess = new InvestorAccess();

        InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }
        InteractionRequest<INotification> NotificationRequest { get; set; }
        public ICommand CloseTabCommand { get; set; }

        public ImportCashFlowsViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            eventAggregator.GetEvent<ImportEvent>().Subscribe(OnImportEvent);
            CloseTabCommand = new DelegateCommand(OnCloseTab);
        }
        private bool readFile=false;
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;

        public bool ReadFile
        {
            get { return this.readFile; }
            set { SetProperty(ref readFile, value); }
        }

        private ObservableCollection<string> errorMessages;

        public ObservableCollection<string> ErrorMessages
        {
            get { return this.errorMessages; }
            set { SetProperty(ref errorMessages, value); }
        }

        private string totalNumberOfRecords = string.Empty;

        public string TotalNumberOfRecords
        {
            get { return this.totalNumberOfRecords; }
            set { SetProperty(ref totalNumberOfRecords, value); }
        }

        private string currentNumberOfRecords = string.Empty;

        public string CurrentNumberOfRecords
        {
            get { return this.currentNumberOfRecords; }
            set { SetProperty(ref currentNumberOfRecords, value); }
        }
  
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            fileInfo = navigationContext.Parameters["FileInfo"] as FileInfo;
            commitments = navigationContext.Parameters["PsPlus"] as ObservableCollection<ImportCommitment>;

            ReadFile = true;
            ReadCashFlows();
            TotalNumberOfRecords = $"Die Datei enthält {cashFlows.Count:n0} Datensätze.";
            var task = ImportCashFlows();
        }

        private async Task  ImportCashFlows()
        {
            try
            {
                await Task.Run(() => InsertCashFlowsAsync());
                ReadFile = false;
            }
            catch (Exception ex)
            {
                eventAggregator.GetEvent<ImportEvent>().Publish($"Fehler beim Einlesen der CashFlows. {ex.Message} "); 
            }           
        }
        private void OnCloseTab()
        {
            var activeView = regionManager.Regions[RegionNames.TabControlRegion].ActiveViews.ElementAt(0);
            if (activeView == null) return;
            regionManager.Regions[RegionNames.TabControlRegion].Remove(activeView);
        }


        private void InsertCashFlowsAsync()
        {
            //the first item of cashFlows is read
            //select all cashFlows with the same porfolionumber and investornumber
            //remove all selected records from cashFlows
            //  try to find commitment in commitments
            //  if found:
            //  read all InvestorCashFlows from the Database
            int counter = 0;
            int previousCounter = 0;
            while (cashFlows.Count > 0)
            {
                ImportCashFlow cf = cashFlows.ElementAt(0);
                List<ImportCashFlow> specificCashFlows = cashFlows.Where(c => c.InvestorNumber == cf.InvestorNumber && c.PeFundNumber == cf.PeFundNumber).ToList();
                DateTime lastCashFlowDate = specificCashFlows.Max(c => c.CashFlowDate);
                DateTime firstCashFlowDate = specificCashFlows.Min(c => c.CashFlowDate);

                for( int i =0; i<specificCashFlows.Count; i++)
                {
                    cashFlows.RemoveAt(0);
                }
                counter += specificCashFlows.Count;
                if (counter-previousCounter>100)
                {
                    CurrentNumberOfRecords = $"Es wurden {counter:n0} Datensätze verarbeitet.";
                    previousCounter = counter;
                }

                eventAggregator.GetEvent<ImportInformationEvent>().Publish(new ImportInformation()
                {
                    ImportName = "Cashflows",
                    Information = $"Der Import der Cashflows ist gestartet"
                });

                List<InvestorCashFlow> dbCashFlows = new List<InvestorCashFlow>();
                ImportCommitment commitment = commitments.FirstOrDefault(c => c.InvestorNumber == cf.InvestorNumber && c.PeFundNumber == cf.PeFundNumber);
                if (commitment != null)
                {
                    if (commitment.InvestorCommitmentId == 0) continue;     // InvestorCommitmentId =0 if commitment is not in the database
                                                                            // => no CashFlows are inserted
                    dbCashFlows = investorAccess.GetCashFlowsForCommitment(commitment.InvestorCommitmentId);
                }
                else
                {
                    continue;       // if commitment is not found leave database unchanged
                }
                foreach(ImportCashFlow import in specificCashFlows)
                {
                    if (import.PsPlusId == 0) import.PsPlusId = 1;  // there is no PSPlusId for transaction before client starts with HQTrust; default Id =1
                    InvestorCashFlow foundCf = dbCashFlows.FirstOrDefault(c => c.PsPlusId == import.PsPlusId);

                    //if a cashflow with a corresponding PsPlusId is found ==> read next Cashflow
                    if (foundCf != null)
                    {
                        dbCashFlows.Remove(foundCf);
                        continue;
                    }

                    // if not found try to find a cashFlow with the same Date (+- 3 days)
                    InvestorCashFlow selectedCashFlow = null;
                    foreach (InvestorCashFlow icf in dbCashFlows)
                    {
                        TimeSpan span = icf.EffectiveDate.Subtract(import.CashFlowDate);
                        if (Math.Abs(span.Days) > 3) continue;
                        if (icf.CashFlowAmount == import.AmountPeFundCurrency)
                        {
                            icf.PsPlusId = import.PsPlusId;
                            selectedCashFlow = icf;
                            break;
                        }
                    }
                    if (selectedCashFlow!=null)
                    {
                        //update CashFlow
                        // remove CashFlow from dbCashFlows
                        selectedCashFlow.PsPlusId = import.PsPlusId;
                        selectedCashFlow.CapitalGain = import.CapitalGain;
                        selectedCashFlow.CashFlowAmount = import.AmountPeFundCurrency;
                        selectedCashFlow.CashFlowAmountInInvestorCurrency = import.AmountInvestorCurrency;
                        selectedCashFlow.Dividends = import.Dividends;
                        selectedCashFlow.EffectiveDate = import.CashFlowDate;
                        selectedCashFlow.PartnershipExpenses = import.PartnershipExpenses;
                        selectedCashFlow.RecallableAmount = import.RecallableAmount;
                        selectedCashFlow.ReturnOfCapital = import.ReturnOfCapital;
                        selectedCashFlow.WithholdingTax = import.WithholdingTax;

                        try
                        {
                            investorAccess.UpdateInvestorCashFlow(selectedCashFlow);
                        }
                        catch (Exception ex)
                        {
                            eventAggregator.GetEvent<ImportEvent>().Publish($"Fehler beim Ändern eines CashFlows: {ex.Message}");
                        }

                      
                    }
                    else
                    {
                        //in der Datenbank wurde kein CashFlow gefunden => neuen CashFlow eintragen
                        selectedCashFlow = new InvestorCashFlow();
                        selectedCashFlow.InvestorCommitmentId = commitment.InvestorCommitmentId;
                        selectedCashFlow.CommitmentAmount = commitment.Commitment;
                        selectedCashFlow.PsPlusId = import.PsPlusId;
                        selectedCashFlow.CapitalGain = import.CapitalGain;
                        selectedCashFlow.CashFlowAmount = import.AmountPeFundCurrency;
                        selectedCashFlow.CashFlowAmountInInvestorCurrency = import.AmountInvestorCurrency;
                        selectedCashFlow.Dividends = import.Dividends;
                        selectedCashFlow.EffectiveDate = import.CashFlowDate;
                        selectedCashFlow.PartnershipExpenses = import.PartnershipExpenses;
                        selectedCashFlow.RecallableAmount = import.RecallableAmount;
                        selectedCashFlow.ReturnOfCapital = import.ReturnOfCapital;
                        selectedCashFlow.WithholdingTax = import.WithholdingTax;
                        if (selectedCashFlow.CashFlowAmount < 0) selectedCashFlow.CashFlowDescription = "Capital Call";
                        else selectedCashFlow.CashFlowDescription = "Distribution";
                        if (selectedCashFlow.CashFlowAmount < 0) selectedCashFlow.CashFlowType = "Capital Call";
                        else selectedCashFlow.CashFlowType = "Distribution";

                        try
                        {
                            investorAccess.UpdateInvestorCashFlow(selectedCashFlow);
                        }
                        catch (Exception ex)
                        {
                            eventAggregator.GetEvent<ImportEvent>().Publish($"Fehler beim Einfügen eines CashFlows: {ex.Message}");                           
                        }
                    }
                }

                // the remaining InvestorCashFlows in dbCashFlows are either after maxDate or need to be removed
                foreach (InvestorCashFlow icf in dbCashFlows)
                {
                    if (icf.EffectiveDate > lastCashFlowDate) continue;
                    // remove CashFlow from database
                }
                eventAggregator.GetEvent<ImportInformationEvent>().Publish(new ImportInformation()
                {
                    ImportName = "CashFlows",
                    Information = $"Es sind noch {cashFlows.Count.ToString()} Cashflows zu verarbeiten"
                });
            }
            CurrentNumberOfRecords = $"Es wurden alle Datensätze verarbeitet.";
        }

        private void OnImportEvent(string obj)
        {
            Application.Current.Dispatcher.BeginInvoke(
                   DispatcherPriority.Normal,
                   new Action(() => ErrorMessages.Add(obj)));
        }

        private void ReadCashFlows()
        {
            cashFlows = new ObservableCollection<ImportCashFlow>();

            //
            // set Culture to en-Us as the csv file is formatted in en-Us
            //

            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            try
            {
                using (var reader = new StreamReader(fileInfo.FullName))
                {

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');

                        ImportCashFlow cashFlow = GetCashFlow(values);
                        if (cashFlow != null) cashFlows.Add(cashFlow);
                    }
                }
               
            }
            catch (Exception ex)
            {
                ConfirmationRequest.Raise(new Confirmation()
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = ex.Message
                }, OnConfirmTryAgain);
            }
            finally
            {
                // ensure that Culture is set back to original culture
                CultureInfo.CurrentCulture = currentCulture;
            }
          
        }

        private void OnConfirmTryAgain(IConfirmation obj)
        {
            if (obj.Confirmed)
            {
                ReadCashFlows();
                return;
            }
            // CloseTab
        }

        private ImportCashFlow GetCashFlow(string[] values)
        {

            double amount = 0;
            int identification = 0;
            DateTime myDate = DateTime.Now;
            double capitalCall = 0;
            double distribution = 0;

            string pfnr = values[(int)ImportColumns.pfnr];
            string wkn = values[(int)ImportColumns.wkn];
           
            ImportCashFlow cf = new ImportCashFlow();
            cf.PeFundNumber = wkn;
            cf.InvestorNumber = pfnr;

            if (int.TryParse(values[(int)ImportColumns.trxId], out identification))
            {
                cf.PsPlusId = identification;              
            }

            if (double.TryParse(values[(int)ImportColumns.flowFund], out amount))
            {
                cf.AmountPeFundCurrency = amount;
            }

            if (double.TryParse(values[(int)ImportColumns.flowInvestor], out amount))
            {
                cf.AmountInvestorCurrency = amount;
            }

            if (double.TryParse(values[(int)ImportColumns.trx50021CapitalGainFund], out amount))
            {
                cf.CapitalGain = amount;
            }

            if (DateTime.TryParse(values[(int)ImportColumns.effectiveDate], out myDate))
            {
                cf.CashFlowDate = myDate.Date;            
            }

            if (double.TryParse(values[(int)ImportColumns.trx50020IncomeFund], out amount))
            {
                cf.Dividends = amount;
            }

            if (double.TryParse(values[(int)ImportColumns.trx50021ReturnOfCapitalFund], out amount))
            {
                cf.ReturnOfCapital = amount;
            }

            if (double.TryParse(values[(int)ImportColumns.trx50020TaxesFund], out amount))
            {
                cf.WithholdingTax = amount;
            }
            if (double.TryParse(values[(int)ImportColumns.trx50023CancellRecallableFund], out amount))
            {
                cf.RecallableAmount = amount;
            }

            if (double.TryParse(values[(int)ImportColumns.trx50032RecallableFund], out amount))
            {
                cf.RecallableAmount = amount;
            }

            if (double.TryParse(values[(int)ImportColumns.trx50020IncomeFund], out amount))
            {
                cf.Dividends = amount;
            }
            if (DateTime.TryParse(values[(int)ImportColumns.trxInputDate], out myDate))
            {
                cf.InputDate = myDate.Date;
            }
            if (double.TryParse(values[(int)ImportColumns.trx50000CallFund], out capitalCall))
            {

            }
            if (double.TryParse(values[(int)ImportColumns.trx50021DistributionFund], out distribution))
            {

            }  
            cf.ExchangeRate = Math.Round(cf.AmountPeFundCurrency / cf.AmountInvestorCurrency, 6);
  
            return cf;
        }
    }
}
