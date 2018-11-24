using BLL_DataModels;
using BLL_Prism;
using Prism.Regions;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL_DataAccess;
using System.Windows.Input;
using Prism.Commands;
using BLL_Infrastructure;
using System.IO;
using System.Globalization;
using Telerik.Windows.Controls;
using Prism.Interactivity.InteractionRequest;

namespace BLL_Private_Equity.Views
{
    public class ExportInvestorListViewModel: HqtBindableBase, IRegionManagerAware
    {
        public ICommand ReloadCommand { get; set; }
        public ICommand EditInvestorCommand { get; set; }
        public ICommand BrowseCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand GridViewLoadedCommand { get; set; }
        public InteractionRequest<INotification> NotificationRequest { get; set; }

        Telerik.Windows.Controls.RadGridView gridView = new Telerik.Windows.Controls.RadGridView();

        private string exportFileName = "DateiName";

        public string ExportFileName 
        {
            get { return this.exportFileName; }
            set { SetProperty(ref exportFileName, value); }
        }

        private string exportDirectoryName = "Dateipfad";

        public string ExportDirectoryName
        {
            get { return this.exportDirectoryName; }
            set { SetProperty(ref exportDirectoryName, value); }
        }

        private InvestorCommitment investorCommitment;

        public InvestorCommitment SelectedCommitment
        {
            get { return this.investorCommitment; }
            set
            {
                SetProperty(ref investorCommitment, value);
                if (SelectedCommitment == null) CanEditInvestor = false; else CanEditInvestor = true;
            }
        }

        private bool dataLoading = false;

        public bool DataLoading
        {
            get { return this.dataLoading; }
            set { SetProperty(ref dataLoading, value); }
        }

        private bool canEditInvestor = false;

        public bool CanEditInvestor
        {
            get { return this.canEditInvestor; }
            set { SetProperty(ref canEditInvestor, value); }
        }

        private bool canExportData =true;

        public bool CanExportData
        {
            get { return this.canExportData; }
            set { SetProperty(ref canExportData, value); }
        }
        private PeFund fund;

        public PeFund Fund
        {
            get { return this.fund; }
            set { SetProperty(ref fund, value); }
        }

        private ObservableCollection<InvestorCommitment> commitments;

        public ObservableCollection<InvestorCommitment> Commitments
        {
            get { return this.commitments; }
            set { SetProperty(ref commitments, value); }
        }

        public IRegionManager RegionManager { get ; set ; }
        private IRegionManager regionManager;

        public ExportInvestorListViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            TabTitle = "Export Investordaten";

            EditInvestorCommand = new Prism.Commands.DelegateCommand(OnEditInvestor).ObservesCanExecute(() => CanEditInvestor);
            ReloadCommand = new Prism.Commands.DelegateCommand(OnReload);
            ExportCommand = new Prism.Commands.DelegateCommand(OnExport).ObservesCanExecute(() => CanExportData);
            BrowseCommand = new Prism.Commands.DelegateCommand(OnBrowse);
            // an instance of RadGridView is needed to perform export to Excel
            GridViewLoadedCommand = new Prism.Commands.DelegateCommand<object>(OnGridViewLoaded);
            NotificationRequest = new InteractionRequest<INotification>();
            GetDirectory();
        }

        private void OnGridViewLoaded(object obj)
        {
            System.Windows.RoutedEventArgs e = obj as System.Windows.RoutedEventArgs;
            gridView = e.OriginalSource as Telerik.Windows.Controls.RadGridView;
            if (gridView == null)
            {

            }
        }

        private void GetDirectory()
        {
            
            ExportDirectoryName = Properties.Settings.Default.ExportDirectory;
            if (string.IsNullOrEmpty(ExportDirectoryName)) ExportDirectoryName = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);

            DirectoryInfo directoryInfo = new DirectoryInfo(ExportDirectoryName);
            if (directoryInfo.Exists == false)
            {
                ExportDirectoryName = string.Empty;
                CanExportData = false;
            }
        }

        private void OnBrowse()
        {
            if (string.IsNullOrEmpty(ExportDirectoryName)) ExportDirectoryName = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            Telerik.Windows.Controls.RadOpenFolderDialog openFolderDialog = new Telerik.Windows.Controls.RadOpenFolderDialog();
            openFolderDialog.InitialDirectory = ExportDirectoryName;
            openFolderDialog.ShowDialog();
            if (openFolderDialog.DialogResult == true)
            {
                ExportDirectoryName = openFolderDialog.FileName;
                Properties.Settings.Default.ExportDirectory = exportDirectoryName;
                Properties.Settings.Default.Save();
                CanExportData = true;
            }
        }

        private void OnExport()
        {
            string fileName = Path.Combine(ExportDirectoryName, ExportFileName);
            try
            {
                using (System.IO.Stream stream = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    gridView.ExportToXlsx(stream,
                        new Telerik.Windows.Controls.GridViewDocumentExportOptions()
                        {
                            Culture = CultureInfo.CurrentCulture,
                            ShowColumnFooters = true,
                            ShowColumnHeaders = true,
                            ShowGroupFooters = true,
                            ExportDefaultStyles = true,
                        });
                }

            }
            catch (Exception ex)
            {
                NotificationRequest.Raise(new Notification()
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = $"Die Excel-Datei ({fileName}) kann nicht erstellt werden." + Environment.NewLine + Environment.NewLine +
                        ex.Message
                });
                return;
            }
        
            NotificationRequest.Raise(new Notification()
            {
                Title = ApplicationNames.NotificationTitle,
                Content = $"Die Excel-Datei {fileName} wurde erstellt."
            });
        }

        private void OnReload()
        {
            var task = LoadCommitmentssAsync();
        }

        private void OnEditInvestor()
        {
            // Navigate to InvestorDetail
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Investor", SelectedCommitment.Investor);

            regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.InvestorDetails, parameter);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Fund = navigationContext.Parameters["Fund"] as PeFund;
            var task = LoadCommitmentssAsync();

            // create ExportFileName "FundShortName" + YYYYMMDD

            StringBuilder stringBuilder = new StringBuilder(Fund.FundShortName);
            stringBuilder.Append(" " + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + ".xlsx");

            ExportFileName = stringBuilder.ToString();            
        }
        private async Task LoadCommitmentssAsync()
        {
            DataLoading = true;
            Commitments = new ObservableCollection<InvestorCommitment>(await PefundAccess.GetCommitmentsForPeFundAsync(Fund.Id));
           
            DataLoading = false;
            RaisePropertyChanged("Commitments");
            RaisePropertyChanged("Fund");

        }
    }
}
