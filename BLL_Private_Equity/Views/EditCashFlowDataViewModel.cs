using BLL_Prism;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL_Private_Equity.Berechnungen;
using System.Windows.Input;
using Prism.Commands;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;
using BLL_Infrastructure;
using BLL_DataAccess;
using System.IO;
using Xceed.Wpf.Toolkit;
using Telerik.Windows.Documents.Fixed;
using Telerik.Windows.Documents.Fixed.FormatProviders;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.Model;
using BLL_Private_Equity.Events;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using BLL_DataModels;

namespace BLL_Private_Equity.Views
{
    public class EditCashFlowDataViewModel: HqtBindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private InvestorAccess investorAccess = new InvestorAccess();
        private bool isCombinedCashFlow = false;

        private ObservableCollection<InvestorCommitment> investorCommitments;

        public ICollection<InvestorCommitment> NewInvestorCommitments;


        private List<string> templates;

        public List<string> Templates
        {
            get { return this.templates; }
            set { SetProperty(ref templates, value); }
        }



        private WindowState showTestLetter;
        

        public WindowState ShowTestLetter
        {
            get { return this.showTestLetter; }
            set { SetProperty(ref showTestLetter, value); }
        }


        private RadFixedDocument testLetterPath;


        public RadFixedDocument TestLetterPath
        {
            get { return this.testLetterPath; }
            set { SetProperty(ref testLetterPath, value); }
        }


        private double distributionDifference;

        public double DistributionDifference
        {
            get { return this.distributionDifference; }
            set { SetProperty(ref distributionDifference, value); }
        }

        private double netDistribution;

        public double NetDistribution
        {
            get { return this.netDistribution; }
            set { SetProperty(ref netDistribution, value); }
        }

        private bool isCreatingDocuments = false;

        public bool IsCreatingDocuments
        {
            get { return this.isCreatingDocuments; }
            set { SetProperty(ref isCreatingDocuments, value); }
        }

        private bool dataInputDone = false;

        public bool DataInputDone
        {
            get { return this.dataInputDone; }
            set { SetProperty(ref dataInputDone, value); }
        }



        private CashFlowInformation cashFlowInformation;

        public CashFlowInformation CashFlowInformation
        {
            get { return this.cashFlowInformation; }
            set { SetProperty(ref cashFlowInformation, value); }
        }

        public ICommand PrintTestLetterCommand { get; set; }
        public ICommand GotoNextStepCommand { get; set; }
      
        public InteractionRequest<INotification> NotificationRequest { get; set; }
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }


        public EditCashFlowDataViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;

            
            PrintTestLetterCommand = new DelegateCommand(OnPrintTestLetter).ObservesCanExecute(() => DataInputDone);
            GotoNextStepCommand = new DelegateCommand(OnGotoNextStep).ObservesCanExecute(() => DataInputDone);
           
            NotificationRequest = new InteractionRequest<INotification>();
            ConfirmationRequest = new InteractionRequest<IConfirmation>();
        }

       

        private void OnGotoNextStep()
        {
            // check cashflows whether another cashflow for the same fund has already been entered
            // checkFlowexists returns true if a cashflow exists for date and fund

            if (PefundAccess.CashFlowExists(CashFlowInformation.EffectiveDate, CashFlowInformation.Fund.Id) ==false)
            {
                CashFlowInformation.CashFlowDataEntered = true;
                PrepareCashFlow prepareCashFlow = new PrepareCashFlow()
                {
                    cfInfo = cashFlowInformation
                };
                eventAggregator.GetEvent<PrepareCashFlowEvent>().Publish(prepareCashFlow);
            }
            else
            {
                ConfirmationRequest.Raise(new Confirmation()
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = "Für dieses Datum wurde bereits ein Cashflow erfasst. Wollen Sie einen weiteren CashFlow erfassen?"
                }, OnFurtherCashflow);
            }          
        }

        private void OnFurtherCashflow(IConfirmation obj)
        {
           if (obj.Confirmed)
            {
                CashFlowInformation.CashFlowDataEntered = true;
                PrepareCashFlow prepareCashFlow = new PrepareCashFlow()
                {
                    cfInfo = cashFlowInformation
                };
                eventAggregator.GetEvent<PrepareCashFlowEvent>().Publish(prepareCashFlow);
            }
        }

        private void OnPrintTestLetter()
        {
            if (string.IsNullOrEmpty(CashFlowInformation.WordDocument)) return;
            // prepare Test letter

            ShowTestLetter = WindowState.Open;
            IsCreatingDocuments = true;
            FileInfo fileInfo = WordTemplateHelper.GetWordFileInfo(CashFlowInformation.WordDocument);
            string sourcePath = fileInfo.FullName;
            string testDocx = Path.Combine(fileInfo.DirectoryName, "TestLetter.docx");
            string testPdf = Path.Combine(fileInfo.DirectoryName, "TestLetter.pdf");

            CashFlowInformation.DetailSummary.Investor = investorAccess.GetSampleInvestor();
            CashFlowInformation.DetailSummary.Reference = "BeispielInvestor";
            //CreateClientLetter letter = new CreateClientLetter(sourcePath, testDocx, CashFlowInformation, CashFlowInformation.DetailSummary);
            try
            {
                CreateClientLetterTelerik letter = new CreateClientLetterTelerik(sourcePath, testDocx, CashFlowInformation, CashFlowInformation.DetailSummary);
            }
            catch (Exception ex)
            {
                NotificationRequest.Raise(new Notification()
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = ex.Message
                });
                return;
            }
           

            MemoryStream stream = new MemoryStream();

            using (Stream input = File.OpenRead(testPdf))
            {
                input.CopyTo(stream);
            }

            try
            {
                FormatProviderSettings settings = new FormatProviderSettings(ReadingMode.AllAtOnce);
                PdfFormatProvider provider = new PdfFormatProvider(stream, settings);
                RadFixedDocument doc = provider.Import();
                TestLetterPath = doc;
            }
            catch (Exception ex)
            {

                NotificationRequest.Raise(new Notification()
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = "Beim Lesen der pdf Datei ist ein Fehler aufgetreten."
                });
            }
          
            IsCreatingDocuments = false;

            //DirectoryHelper.DeleteFile(testDocx);
            //DirectoryHelper.DeleteFile(testPdf);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            CashFlowInformation = navigationContext.Parameters["Info"] as CashFlowInformation;
            RaisePropertyChanged("CashFlowInformation");

            if (CashFlowInformation.DetailSummary.CashFlowAmountCall != 0 && CashFlowInformation.DetailSummary.CashFlowAmountDistribution != 0)
                isCombinedCashFlow = true;
            else isCombinedCashFlow = false;

            if (CashFlowInformation.DetailSummary.CashFlowAmount != 0 && DistributionDifference == 0 && (!string.IsNullOrEmpty(CashFlowInformation.WordDocument))) DataInputDone = true;

            CashFlowInformation.PropertyChanged -= CashFlowInformation_PropertyChanged;
            CashFlowInformation.DetailSummary.PropertyChanged -= DetailSummary_PropertyChanged;

            CashFlowInformation.PropertyChanged += CashFlowInformation_PropertyChanged;
            CashFlowInformation.DetailSummary.PropertyChanged += DetailSummary_PropertyChanged;
            RaisePropertyChanged("Templates");
            TabTitle = "Cash Flow";

            Templates = WordTemplateHelper.GetWordTemplates();
            if (Templates.Count == 0)
            {
                NotificationRequest.Raise(new Notification()
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = "Es sind noch keine Word-Templates für die Cash-Flow-Briefe vorhanden"
                });
            }

        }

        private void DetailSummary_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName=="CashFlowAmountCall")
            {
                if (CashFlowInformation.DetailSummary.CashFlowAmountCall != 0 && CashFlowInformation.DetailSummary.CashFlowAmountDistribution !=0)
                {
                    if (!isCombinedCashFlow)
                    {
                        ConfirmationRequest.Raise(new Confirmation()
                        {
                            Title = ApplicationNames.NotificationTitle,
                            Content = "Es ist bereits ein Ausschüttungsbetrag erfasst. Handelt es sich um einen kombinierten CashFlow?"
                        }, OnConfirmationResponse);
                    }
                }
            }
            if (e.PropertyName == "CashFlowAmountDistribution")
            {
                if (CashFlowInformation.DetailSummary.CashFlowAmountCall != 0 && CashFlowInformation.DetailSummary.CashFlowAmountDistribution != 0)
                {
                    if (!isCombinedCashFlow)
                    {
                        ConfirmationRequest.Raise(new Confirmation()
                        {
                            Title = ApplicationNames.NotificationTitle,
                            Content = "Es ist bereits ein Capitla Call erfasst. Handelt es sich um einen kombinierten CashFlow?"
                        }, OnConfirmationResponse);
                    }
                }
            }

            if (CashFlowInformation.DetailSummary.CashFlowAmountCall == 0 || CashFlowInformation.DetailSummary.CashFlowAmountDistribution == 0) isCombinedCashFlow = false;

            if (CashFlowInformation.DetailSummary.CashFlowAmountCall != 0) CashFlowInformation.CashFlowType = "Capital Call";
            if (CashFlowInformation.DetailSummary.CashFlowAmountDistribution != 0) CashFlowInformation.CashFlowType = "Distribution";
            if (CashFlowInformation.DetailSummary.CashFlowAmountCall != 0 && CashFlowInformation.DetailSummary.CashFlowAmountDistribution != 0)
                CashFlowInformation.CashFlowType = "Combined CashFlow";

            DistributionDifference = 0;
            DistributionDifference = CashFlowInformation.DetailSummary.CashFlowAmountDistribution +
                CashFlowInformation.DetailSummary.WithholdingTax -
                CashFlowInformation.DetailSummary.ReturnOfCapital -
                CashFlowInformation.DetailSummary.CapitalGain -
                CashFlowInformation.DetailSummary.Dividends -
                CashFlowInformation.DetailSummary.RecallableAmount;

            CashFlowInformation.DetailSummary.CashFlowAmount = Math.Abs(
                Math.Abs(CashFlowInformation.DetailSummary.CashFlowAmountCall) -
                CashFlowInformation.DetailSummary.CashFlowAmountDistribution);

            NetDistribution = CashFlowInformation.DetailSummary.CashFlowAmountDistribution + CashFlowInformation.DetailSummary.WithholdingTax;

            if (CashFlowInformation.DetailSummary.CashFlowAmount != 0 && DistributionDifference == 0 && (!string.IsNullOrEmpty(CashFlowInformation.WordDocument)))
            {
                DataInputDone = true;
                WriteInformationToTextFile();
            }
               
            
        }

        private void OnConfirmationResponse(IConfirmation obj)
        {
           if (obj.Confirmed)
            {
                isCombinedCashFlow = true;
            }
        }

        private void CashFlowInformation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (CashFlowInformation.DetailSummary.CashFlowAmount != 0 && DistributionDifference == 0 && (!string.IsNullOrEmpty(CashFlowInformation.WordDocument)))
            {
                DataInputDone = true;
                WriteInformationToTextFile();
            }
        }
        private void WriteInformationToTextFile()
        {
            //string output = JsonConvert.SerializeObject(cashFlowInformation);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            using (StreamWriter sw = new StreamWriter(CashFlowInformation.FileName))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, cashFlowInformation);
                }
            }
        }
    }
}
