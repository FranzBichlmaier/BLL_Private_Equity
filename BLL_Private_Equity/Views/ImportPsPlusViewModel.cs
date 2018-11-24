using BLL_DataModels;
using BLL_Private_Equity.Events;
using BLL_Infrastructure;
using BLL_Prism;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.Windows.Controls;


namespace BLL_Private_Equity.Views
{
    public class ImportPsPlusViewModel: HqtBindableBase, IConfirmNavigationRequest
    {
        private bool? workDone = null;
        private int numberOfProcesses = 0;
        private SubscriptionToken token;
        private FileInfo currencyInfo;
        private FileInfo commitmentInfo;
        private FileInfo cashFlowInfo;
        private ObservableCollection<ImportCommitment> importCommitments = null; 


        private string currencyStatus ="Verarbeitung wurde noch nicht gestartet";

        public string CurrencyStatus
        {
            get { return this.currencyStatus; }
            set { SetProperty(ref currencyStatus, value); }
        }

        private string commitmentStatus = "Verarbeitung wurde noch nicht gestartet";

        public string CommitmentStatus
        {
            get { return this.commitmentStatus; }
            set { SetProperty(ref commitmentStatus, value); }
        }

        private string cashFlowStatus = "Verarbeitung wurde noch nicht gestartet";

        public string CashFlowStatus
        {
            get { return this.cashFlowStatus; }
            set { SetProperty(ref cashFlowStatus, value); }
        }

        private string currencyFileName;

        public string CurrencyFileName
        {
            get { return this.currencyFileName; }
            set { SetProperty(ref currencyFileName, value); }
        }

        private string commitmentFileName;

        public string CommitmentFileName
        {
            get { return this.commitmentFileName; }
            set { SetProperty(ref commitmentFileName, value); }
        }

        private string cashFlowFileName;
        private readonly IEventAggregator eventAggregator;
        private readonly IUnityContainer container;
        private readonly IRegionManager regionManager;

        public string CashFlowFileName
        {
            get { return this.cashFlowFileName; }
            set { SetProperty(ref cashFlowFileName, value); }
        }

        public ICommand FileOpenCommand { get; set; }
        public ICommand StartImportCommand { get; set; }

        public InteractionRequest<INotification> NotificationRequest { get; set; }


        public ImportPsPlusViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            this.eventAggregator = eventAggregator;
           
            this.regionManager = regionManager;
            NotificationRequest = new InteractionRequest<INotification>();

            FileOpenCommand = new DelegateCommand<object>(OnFileOpen);
            StartImportCommand = new DelegateCommand<object>(OnStartImport);

            token = eventAggregator.GetEvent<ImportInformationEvent>().Subscribe(OnInformationChanged);
            


            //container.RegisterType<object, ImportCurrencyRates>(ViewNames.ImportCurrencyRates);
            //container.RegisterType<object, ImportCommitments>(ViewNames.ImportCommitments);
            //container.RegisterType<object, ImportCashFlows>(ViewNames.ImportCashFlows);
            //container.RegisterType<object, ImportNav>(ViewNames.ImportNav);
        }

        private void OnInformationChanged(ImportInformation import)
        {
            if (import.ImportName.Equals("Currency")) CurrencyStatus = import.Information;
            if (import.ImportName.Equals("Commitment"))
            {
                CommitmentStatus = import.Information;
                if (import.Object != null) importCommitments = import.Object as ObservableCollection<ImportCommitment>;
            }
            if (import.ImportName.Equals("Cashflows")) CashFlowStatus = import.Information;
            if (import.ImportName.Equals("Ended")) numberOfProcesses--;
            if (numberOfProcesses == 0) workDone = true;
        }

        private void OnStartImport(object obj)
        {
            string parameter = (string)obj;
            if (parameter.Equals("Currency"))
            {
                currencyInfo = new FileInfo(CurrencyFileName);
                if (currencyInfo.Exists)
                {
                    Properties.Settings.Default.CurrencyFile = CurrencyFileName;
                    Properties.Settings.Default.Save();

                    NavigationParameters p = new NavigationParameters();
                    p.Add("FileInfo", currencyInfo);


                    regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ImportCurrencyRates, p);
                    workDone = false;
                    numberOfProcesses++;
                    
                }
            }
            if (parameter.Equals("Commitment"))
            {
                commitmentInfo = new FileInfo(CommitmentFileName);
                if (commitmentInfo.Exists)
                {
                    Properties.Settings.Default.CommitmentFile = CommitmentFileName;
                    Properties.Settings.Default.Save();

                    NavigationParameters p = new NavigationParameters();
                    p.Add("FileInfo", commitmentInfo);


                    regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ImportCommitments, p);
                    workDone = false;
                    numberOfProcesses++;
                }
            }
            if (parameter.Equals("Cashflows"))
            {
                if (importCommitments == null)
                {
                    NotificationRequest.Raise(new Notification()
                    {
                        Title = ApplicationNames.NotificationTitle,
                        Content = "Der Import von Commitments wurde nocht nicht gestartet, bzw. ist noch nicht beendet."
                    });
                    return;
                }
                cashFlowInfo = new FileInfo(CashFlowFileName);
                if (cashFlowInfo.Exists)
                {
                    Properties.Settings.Default.CashFlowFile = CashFlowFileName;
                    Properties.Settings.Default.Save();

                    NavigationParameters p = new NavigationParameters();
                    p.Add("FileInfo", cashFlowInfo);
                    p.Add("PsPlus", importCommitments);

                    // start processing
                   
                    regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ImportCashFlows, p);
                    workDone = false;
                }
            }
        }

        private void OnFileOpen(object obj)
        {
            string parameter = (string)obj;
            RadOpenFileDialog openFileDialog = new RadOpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            if (parameter.Equals("Currency"))
            {
                if (currencyInfo != null && currencyInfo.Exists) openFileDialog.InitialDirectory = currencyInfo.DirectoryName;
            }
            if (parameter.Equals("Commitment"))
            {
                if (commitmentInfo != null && commitmentInfo.Exists) openFileDialog.InitialDirectory = commitmentInfo.DirectoryName;
            }
            if (parameter.Equals("Cashflows"))
            {
                if (cashFlowInfo != null && cashFlowInfo.Exists) openFileDialog.InitialDirectory = cashFlowInfo.DirectoryName;
            }
            openFileDialog.ShowDialog();
            if (openFileDialog.DialogResult == true)
            {
                if (parameter.Equals("Currency")) CurrencyFileName = openFileDialog.FileName;
                if (parameter.Equals("Commitment")) CommitmentFileName = openFileDialog.FileName;
                if (parameter.Equals("Cashflows")) CashFlowFileName = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// the ConfirmNavigationRequest controls whether the view can be closed
        /// if continuationCallback uses false as Parameter it can't be closed
        /// workDone is set to true once the excelsheet is completely processed
        /// </summary>
        /// <param name="navigationContext"></param>
        /// <param name="continuationCallback"></param>
        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (workDone==null || workDone == false) continuationCallback(false);
            if (workDone == true) continuationCallback(true);            
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //eventAggregator.GetEvent<ImportInformationEvent>().Unsubscribe(token);
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (workDone != null) return;
            // workdone = false prevents the Tab from Closing
            // workdone is set to false once an import process has been startet and set to true when it has been ended;
            workDone = true;
            TabTitle = "Import Übersicht";

            CurrencyFileName = Properties.Settings.Default.CurrencyFile;
            CommitmentFileName = Properties.Settings.Default.CommitmentFile;
            CashFlowFileName = Properties.Settings.Default.CashFlowFile;

            if (!string.IsNullOrEmpty(CurrencyFileName)) currencyInfo = new FileInfo(CurrencyFileName);
            if (!string.IsNullOrEmpty(CommitmentFileName)) commitmentInfo = new FileInfo(CommitmentFileName);
            if (!string.IsNullOrEmpty(CashFlowFileName)) cashFlowInfo = new FileInfo(CashFlowFileName);

        }
    }
}
