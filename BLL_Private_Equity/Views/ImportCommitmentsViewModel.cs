using BLL_DataModels;
using BLL_Infrastructure;
using BLL_Prism;
using BLL_Private_Equity.Events;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;
using BLL_Private_Equity.Berechnungen;
using BLL_DataAccess;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Prism.Commands;
using System.Windows.Threading;

namespace BLL_Private_Equity.Views
{
    public class ImportCommitmentsViewModel: HqtBindableBase
    {
        Workbook workbook;
        readonly Sheet sheet;
        FileInfo fileInfo;
        ExcelHelperFunctions sheetFunctions;
        List<Currency> currencies = (List<Currency>)ComboboxLists.GetCurrencies();
        InvestorAccess investorAccess = new InvestorAccess();
        List<string> fundList = new List<string>();
        List<string> investorList = new List<string>();
        Sheet navSheet = null;


        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }
        public InteractionRequest<INotification> NotificationRequest { get; set; }
        public ICommand ShowMissingFundsCommand { get; set; }
        public ICommand ShowMissingInvestorsCommand { get; set; }
        public ICommand ShowDifferentCommitmentsCommand { get; set; }
        public ICommand ShowAddedCommitmentsCommand { get; set; }
        public ICommand ClearFilterCommand { get; set; }
        public ICommand AddMissingItemsCommand { get; set; }
        public ICommand StartImportCommand { get; set; }

        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly IRegion region;
        private bool canShowMissingFunds = false;

        public bool CanShowMissingFunds
        {
            get { return this.canShowMissingFunds; }
            set { SetProperty(ref canShowMissingFunds, value); }
        }

        private bool canShowMissingInvestors = false;

        public bool CanShowMissingInvestors
        {
            get { return this.canShowMissingInvestors; }
            set { SetProperty(ref canShowMissingInvestors, value); }
        }

        private bool canShowDifferentCommitments=false;

        public bool CanShowDifferentCommitments
        {
            get { return this.canShowDifferentCommitments; }
            set { SetProperty(ref canShowDifferentCommitments, value); }
        }

        private bool  canShowAddedCommitments;

        public bool  CanShowAddedCommitments
        {
            get { return this.canShowAddedCommitments; }
            set { SetProperty(ref canShowAddedCommitments, value); }
        }


        private ObservableCollection<ImportCommitment> iCommitments;

        public ObservableCollection<ImportCommitment>ICommitments
        {
            get { return this.iCommitments; }
            set { SetProperty(ref iCommitments, value); }
        }
        public ICollectionView ImportCommitments { get; set; } 

        private bool readExcel = false;

        public bool ReadExcel
        {
            get { return this.readExcel; }
            set { SetProperty(ref readExcel, value); }
        }

        private bool canAddMissingItems = false;

        public bool CanAddMissingItems
        {
            get { return this.canAddMissingItems; }
            set { SetProperty(ref canAddMissingItems, value); }
        }

        private bool canStartImport = false;

        public bool CanStartImport
        {
            get { return this.canStartImport; }
            set { SetProperty(ref canStartImport, value); }
        }

       
        public ImportCommitmentsViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            //this.region = region;
            ConfirmationRequest = new InteractionRequest<IConfirmation>();
            NotificationRequest = new InteractionRequest<INotification>();
            ShowMissingFundsCommand = new DelegateCommand(OnShowMissingFunds).ObservesCanExecute(() => CanShowMissingFunds);
            ShowMissingInvestorsCommand = new DelegateCommand(OnShowMissingInvestors).ObservesCanExecute(() => CanShowMissingInvestors);
            ShowDifferentCommitmentsCommand = new DelegateCommand(OnShowDifferentCommitments).ObservesCanExecute(() => CanShowDifferentCommitments);
            ShowAddedCommitmentsCommand = new DelegateCommand(OnShowAddedCommitments).ObservesCanExecute(() => CanShowAddedCommitments);
            AddMissingItemsCommand = new DelegateCommand(OnAddMissingItems).ObservesCanExecute(() => CanAddMissingItems);
            StartImportCommand = new DelegateCommand(OnStartImport).ObservesCanExecute(() => CanStartImport);
            ClearFilterCommand = new DelegateCommand(OnClearFilter);
        }

        private async void OnStartImport()
        {
            ICommitments = await Task.Run(() => CreateImportCommitmentsAsync());
            await Task.Run(() => ReadCommitmentsAsync());
            ImportCommitments = CollectionViewSource.GetDefaultView(ICommitments);
            RaisePropertyChanged("ImportCommitments");

            if (CanAddMissingItems == false)
            {
                StartNAVImport();
            }
        }

        private void OnAddMissingItems()
        {
            CanAddMissingItems = false;
            foreach (ImportCommitment item in iCommitments)
            {
                if (item.PeFundId ==0 || item.InvestorId==0)
                {
                    if (item.PeFundId ==0)
                    {
                        string found = fundList.FirstOrDefault(p => p == item.PeFundNumber);
                        if (string.IsNullOrEmpty(found))
                        {
                            fundList.Add(item.PeFundNumber);
                            // Fund hinzufügen
                            PeFund newFund = new PeFund()
                            {
                                FundHqTrustNumber = item.PeFundNumber,
                                FundName = item.PeFundName,
                                CurrencyId = item.PeFundCurrencyId
                            };
                            try
                            {
                                PefundAccess.InsertPeFund(newFund);
                            }
                            catch (Exception ex)
                            {
                                NotificationRequest.Raise(new Notification()
                                {
                                    Title = ApplicationNames.NotificationTitle,
                                    Content = $"Fehler beim Einfügen eines Funds. {ex.Message}"
                                });
                            }
                           
                        }
                    }
                    if (item.InvestorId ==0)
                    {
                        string found = investorList.FirstOrDefault(p => p == item.InvestorNumber);
                        if (string.IsNullOrEmpty(found))
                        {
                            investorList.Add(item.InvestorNumber);
                            
                            Investor newInvestor = new Investor()
                            {
                                InvestorHqTrustAccount = item.InvestorNumber,
                                InvestorReference = "Hinzugefügt "+ item.InvestorNumber,
                                CurrencyId = item.InvestorCurrencyId
                            };
                            try
                            {
                                investorAccess.InsertInvestor(newInvestor);
                            }
                            catch (Exception ex)
                            {
                                NotificationRequest.Raise(new Notification()
                                {
                                    Title = ApplicationNames.NotificationTitle,
                                    Content = $"Fehler beim Einfügen eines Investors. {ex.Message}"
                                });
                            }
                        }
                    }
                }
            }
            CanStartImport = true;
            eventAggregator.GetEvent<InvestorCollectionActionEvent>().Publish(new InvestorCollectionAction()
            {
                action = CollectionAction.reload,
                investor = null
            });
            eventAggregator.GetEvent<PeFundCollectionActionEvent>().Publish(new PeFundCollectionAction()
            {
                action = CollectionAction.reload,
                fund = null
            });
        }

        private void OnClearFilter()
        {
            ImportCommitments.Filter = null;
            // Clear GroupDescriptors
            ImportCommitments.GroupDescriptions.Clear();
        }

        private void OnShowAddedCommitments()
        {
            ImportCommitments.Filter = null;
            ImportCommitments.Filter = item =>
            {
                ImportCommitment c = item as ImportCommitment;
                return c.CommitmentsAdded == true;
            };
            // Clear GroupDescriptors
            ImportCommitments.GroupDescriptions.Clear();
        }

        private void OnShowDifferentCommitments()
        {
            ImportCommitments.Filter = null;
            ImportCommitments.Filter = item =>
            {
                ImportCommitment c = item as ImportCommitment;
                return c.HqpeCommitment != 0;
            };
            // Clear GroupDescriptors
            ImportCommitments.GroupDescriptions.Clear();
        }

        private void OnShowMissingInvestors()
        {
            ImportCommitments.Filter = null;
            ImportCommitments.Filter = item =>
            {
                ImportCommitment c = item as ImportCommitment;
                return c.InvestorId == 0;
            };
            // Clear GroupDescriptors and add group by investor
            ImportCommitments.GroupDescriptions.Clear();
            ImportCommitments.GroupDescriptions.Add(new PropertyGroupDescription("InvestorNumber"));
        }

        private void OnShowMissingFunds()
        {            
            ImportCommitments.Filter = (MissingFundsFilter);
            ImportCommitments.Refresh();
            // Clear GroupDescriptors and add group by fund
            ImportCommitments.GroupDescriptions.Clear();
            ImportCommitments.GroupDescriptions.Add(new PropertyGroupDescription("PeFundNumber"));
        }

        private bool MissingFundsFilter(object obj)
        {
            ImportCommitment c = obj as ImportCommitment;
            if (c.PeFundId == 0)
                return true;
            return false;
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            fileInfo = navigationContext.Parameters["FileInfo"] as FileInfo;
            TabTitle = "Import Commitments";
            

            eventAggregator.GetEvent<ImportInformationEvent>().Publish(new ImportInformation()
            {
                ImportName = "Commitment",
                Information = "Import wurde gestartet"
            });

            //StartImportProcess();     
          
    

            ReadExcel = true;
            string message = await Task.Run(() => StartImport());            
            ReadExcel = false;

            if (string.IsNullOrEmpty(message))
            {
                if (CanAddMissingItems == false)
                {
                    //start import NAVs
                    StartNAVImport();
                }
                return;
            }
            NotificationRequest.Raise(new Notification()
            {
                Title = ApplicationNames.NotificationTitle,
                Content = message
            });
            CloseThisTab();
        }

      private async Task<string> StartImport()
        {
            string message =  await StartProcessAsync();
            return message;
        }

        private async Task<string> StartProcessAsync()
        {
            try
            {
                workbook = await Task.Run(() =>OpenExcelFile());
                ICommitments = await Task.Run(() => CreateImportCommitmentsAsync());

                Sheet cSheet = workbook.Sheets["Commitment"];
                if (cSheet == null)
                {   
                    throw new Exception($"Die Datei {fileInfo.Name} enthält kein Sheet mit dem Namen 'Commitment'.");
                }
                navSheet = workbook.Sheets["NAV"];
                if (navSheet == null)
                {
                    throw new Exception($"Die Datei {fileInfo.Name} enthält kein Sheet mit dem Namen 'NAV'.");
                }

                await Task.Run(() =>ReadCommitmentsAsync());

                eventAggregator.GetEvent<ImportInformationEvent>().Publish(new ImportInformation()
                {
                    ImportName = "Commitment",
                    Information = $"Import wurde beendet. Es wurden {ICommitments.Count.ToString()} Commitments verarbeitet",
                    Object=ICommitments
                });
                eventAggregator.GetEvent<ImportInformationEvent>().Publish(new ImportInformation()
                {
                    ImportName = "Ended"               
                });
                ImportCommitments = CollectionViewSource.GetDefaultView(ICommitments);
                RaisePropertyChanged("ImportCommitments");
  
                    return string.Empty;

                
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        private void StartNAVImport()
        {
            // Start ImportNavs
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Workbook", workbook);
            parameter.Add("Commitments", ICommitments);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ImportNav, parameter);
        }

        private async Task<ObservableCollection<ImportCommitment>> CreateImportCommitmentsAsync()
        {
            ObservableCollection<ImportCommitment> commitments = new ObservableCollection<ImportCommitment>();
           

            var hqtCommitments = await investorAccess.GetHqtCommitmentsAsync();
            foreach(InvestorCommitment commitment in hqtCommitments)
            {
                ImportCommitment import = new ImportCommitment()
                {
                    AsOfDate = DateTime.MinValue,
                    Commitment = commitment.CommitmentAmount,
                    ErrorInformation = string.Empty,
                    FoundInPsPlus = false,
                    InvestorCommitmentId = commitment.Id,
                    InvestorCurrency = commitment.PeFund.Currency.CurrencyShortName,
                    InvestorCurrencyId = (int)commitment.PeFund.CurrencyId,
                    InvestorId = (int)commitment.InvestorId,
                    InvestorNumber = commitment.Investor.InvestorHqTrustAccount,
                    PeFundCurrency = commitment.PeFund.Currency.CurrencyShortName,
                    PeFundCurrencyId = (int)commitment.PeFund.CurrencyId,
                    PeFundName = commitment.PeFund.FundName,
                    PeFundId = commitment.PeFundId,
                    PeFundNumber = commitment.PeFund.FundHqTrustNumber
                };
  
                commitments.Add(import);
            }
            return commitments;
           
        }

        private  Task ReadCommitmentsAsync()
        {
            CanAddMissingItems = false;
            foreach(Worksheet ws in workbook.Sheets)
            {
                if (!ws.Name.Equals("Commitment")) continue;
            
                sheetFunctions = new ExcelHelperFunctions(ws);
                
                for (int row = 1; row < sheetFunctions.RowCount; row++)
                {
                    GetCommitmentRow(row);                    
                   
                }
            }
            return Task.CompletedTask;
        }

        private void GetCommitmentRow(int row)
        {
            ImportCommitment commitment = new ImportCommitment();
            commitment.PeFundNumber = sheetFunctions.GetText(row, 0);
            commitment.PeFundCurrency = sheetFunctions.GetText(row, 1);
            commitment.PeFundCurrencyId = FindCurrencyId(commitment.PeFundCurrency);
            commitment.PeFundName = sheetFunctions.GetText(row, 2);
            commitment.InvestorNumber = sheetFunctions.GetText(row, 3);
            commitment.InvestorCurrency = sheetFunctions.GetText(row, 4);
            commitment.InvestorCurrencyId = FindCurrencyId(commitment.InvestorCurrency);
            commitment.AsOfDate = sheetFunctions.GetDate(row, 5);
            try
            {
                commitment.Commitment = sheetFunctions.GetAmount(row, 6);
            }
            catch (Exception)
            {
                commitment.Commitment = 0;
            }

            if (!int.TryParse(commitment.InvestorNumber.Substring(0, 3), out int test))
            {
                // lines with other content than 3 numerical digits will ignored
                // these lines are for institutional investors
                return;
            }

            ImportCommitment found = ICommitments.
                Where(c => c.PeFundNumber == commitment.PeFundNumber && c.InvestorNumber == commitment.InvestorNumber).FirstOrDefault();
            if (found != null)
            {
                found.FoundInPsPlus = true;
                if (found.Commitment == commitment.Commitment) return ;
                found.ErrorInformation = $"Abweichende Kapitalzusage zwischen PsPlus ({commitment.Commitment:N0}) und HQT Private Equity ({found.Commitment:N0}) ";
                found.HqpeCommitment = found.Commitment;
                CanShowDifferentCommitments = true;
                return;
            }

            // there is a commitment in PS-Plus with no matching commitment in HQPE
            // add a new importCommitment to the importCommitmentCollection 
            // add a InvestorCommitment to the database if PeFund and Investor are found

            ImportCommitment import = new ImportCommitment()
            {
                AsOfDate = commitment.AsOfDate,
                PeFundName = commitment.PeFundName,
                PeFundNumber = commitment.PeFundNumber,
                InvestorNumber = commitment.InvestorNumber,
                InvestorCurrency=commitment.InvestorCurrency,
                InvestorCurrencyId=commitment.InvestorCurrencyId,
                PeFundCurrency= commitment.PeFundCurrency,
                PeFundCurrencyId=commitment.PeFundCurrencyId,
                Commitment = commitment.Commitment,
                FoundInPsPlus=true,
                ErrorInformation = "Es wurde kein Commitment in HQT Private Equity gefunden;"
            };

            // try to find Investor using InvestorNumber; set InvestorId if found
            // try to find PEFund using Beteiligungsnumber; set PeFundId if found

            Investor investor = investorAccess.GetInvestorByHqTrustNumber(import.InvestorNumber);
            if (investor == null)
            {
                import.ErrorInformation += " kein Investor gefunden;";
                import.InvestorId = 0;
                CanShowMissingInvestors = true;
                CanAddMissingItems = true;
            }
            if (investor != null) import.InvestorId = investor.Id;

            PeFund peFund = PefundAccess.GetPeFundByBeteiligungsnummer(import.PeFundNumber);
            if (peFund == null)
            {
                import.ErrorInformation += " kein Fund gefunden;";
                import.PeFundId = 0;
                CanShowMissingFunds = true;
                CanAddMissingItems = true;
            }
            if (peFund != null) import.PeFundId = peFund.Id;

            
            // if investor and fund are found insert investorcommitment

            if (import.PeFundId>0 && import.InvestorId>0)
            {
                // Find Commitment using FundId and InvestorId
                // if found set InvestorCommitmentId  and add ImportCommitment
                // if not add InvestorCommitment

              
                InvestorCommitment newCommitment = new InvestorCommitment()
                {
                    CommitmentAmount = import.Commitment,
                    InvestorId = import.InvestorId,
                    PeFundId = import.PeFundId
                };

                try
                {
                    newCommitment = investorAccess.UpdateInvestorCommitments(newCommitment);
                    import.ErrorInformation = "Das Commitment wurde in die Datenbank eingefügt.";
                    import.InvestorCommitmentId = newCommitment.Id;
                    import.CommitmentsAdded = true;
                    CanShowAddedCommitments = true;
                }
                catch (Exception ex)
                {
                    NotificationRequest.Raise(new Notification()
                    {
                        Title = ApplicationNames.NotificationTitle,
                        Content = ex.Message
                    });
                    CloseThisTab();
                }
               
            }
            ICommitments.Add(import);
            return;  
        }

        private int FindCurrencyId(string peFundCurrency)
        {
            Currency currency = currencies.FirstOrDefault(c => c.CurrencyShortName.Equals(peFundCurrency));
            if (currency == null)
            {
                throw new NotImplementedException();
            }
            return currency.Id;
        }

        private void CloseThisTab()
        {
            var activeView = regionManager.Regions[RegionNames.TabControlRegion].ActiveViews.ElementAt(0);
            if (activeView == null) return;
            regionManager.Regions[RegionNames.TabControlRegion].Remove(activeView);
        }

        private async Task<Workbook> OpenExcelFile()
        {
           
                IWorkbookFormatProvider formatProvider = new XlsxFormatProvider();
                Workbook wb;
                try
                {
                    using (FileStream input = new FileStream(fileInfo.FullName, FileMode.Open))
                    {
                        wb = formatProvider.Import(input);
                        return wb;
                    }
                }
                catch (Exception)
                {
                throw new Exception($"Die Datei {fileInfo.Name} kann nicht geöffnet werden. Sie wird möglicherweise von einem anderen Prozess benutzt. ");
                }
        }
        private void ShowConfirmation(string content)
        {
            ConfirmationRequest.Raise(new Confirmation()
            {
                Title = ApplicationNames.NotificationTitle,
                Content =content                + Environment.NewLine + "Möchten Sie es nochmal versuchen?"
            }, OnConfirmFileOpen);
        }

        private async void OnConfirmFileOpen(IConfirmation obj)
        {
            if (!obj.Confirmed)
            {
                // close tab
                CloseThisTab();
            }
            workbook = await Task.Run(() => OpenExcelFile());
        }
    }
}
