using BLL_DataAccess;
using BLL_DataModels;
using BLL_Private_Equity.Berechnungen;
using BLL_Infrastructure;
using Microsoft.Office.Interop.Outlook;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Documents.FormatProviders.Html;
using Telerik.Windows.Documents.Model;

using OutlookApp = Microsoft.Office.Interop.Outlook.Application;

namespace BLL_Private_Equity.Views
{
    public class CheckLettersViewModel: BLL_Prism.HqtBindableBase
    {

        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private int cashflowNumber = 0;
        private OutlookApp outlookApp = new OutlookApp();
        private Account outlookUsingAccount = null;
        private string appDataDir = string.Empty;
        string billede1 = string.Empty;    // holding image1
        string billede2 = string.Empty;    // holding image2
        HtmlFormatProvider htmlFormatProvider = new HtmlFormatProvider();


        public Account OutlookUsingAccount
        {
            get => outlookUsingAccount; set => SetProperty(ref outlookUsingAccount, value);
        }

        private ObservableCollection<Account> outlookAccounts = new ObservableCollection<Account>();

        public ObservableCollection<Account> OutlookAccounts
        {
            get { return this.outlookAccounts; }
            set { SetProperty(ref outlookAccounts, value); }
        }
        
        public ICommand StartProcessesCommand { get; set; }
        public ICommand AddAttachmentCommand { get; set; }
        public ICommand RemoveAttachmentCommand { get; set; }
        public ICommand EmailBodyLoadedCommand { get; set; }
        public ICommand EmailBodyContentChangedCommand { get; set; }  


        public InteractionRequest<INotification> NotificationRequest { get; set; }

        Telerik.Windows.Controls.RadRichTextBox HtmlBoxEmailBody = new Telerik.Windows.Controls.RadRichTextBox();


        private string emailBodyCall = string.Empty;

        public string EmailBodyCall
        {
            get { return this.emailBodyCall; }
            set { SetProperty(ref emailBodyCall, value); }
        }

        private string emailBodyDistribution=string.Empty;

        public string EmailBodyDistribution
        {
            get { return this.emailBodyDistribution; }
            set { SetProperty(ref emailBodyDistribution, value); }
        }

        private string emailBody=string.Empty;

        public string EmailBody
        {
            get { return this.emailBody; }
            set { SetProperty(ref emailBody, value); }
        }

        private CashFlowInformation cashFlowInformation;

        public CashFlowInformation CashFlowInformation
        {
            get { return this.cashFlowInformation; }
            set { SetProperty(ref cashFlowInformation, value); }
        }

        private bool saveCashFlowTransactions;

        public bool SaveCashFlowTransactions
        {
            get { return this.saveCashFlowTransactions; }
            set { SetProperty(ref saveCashFlowTransactions, value); }
        }

        private string saveCashFlowText;

        public string SaveCashFlowText
        {
            get { return this.saveCashFlowText; }
            set { SetProperty(ref saveCashFlowText, value); }
        }

        private bool saveCashFlowsDone;

        public bool SaveCashFlowsDone
        {
            get { return this.saveCashFlowsDone; }
            set { SetProperty(ref saveCashFlowsDone, value); }
        }
 

        private bool prepareEmails;

        public bool PrepareEmails
        {
            get { return this.prepareEmails; }
            set { SetProperty(ref prepareEmails, value); }
        }

        private bool prepareBankContactLetters;

        public bool PrepareBankContactLetters
        {
            get { return this.prepareBankContactLetters; }
            set { SetProperty(ref prepareBankContactLetters, value); }
        }

        private bool prepareEmailsToBankContacts;

        public bool PrepareEmailsToBankContacts
        {
            get { return this.prepareEmailsToBankContacts; }
            set { SetProperty(ref prepareEmailsToBankContacts, value); }
        }


        private ObservableCollection<FileInfo> attachmentList = new ObservableCollection<FileInfo>();

        public ObservableCollection<FileInfo> AttachmentList
        {
            get { return this.attachmentList; }
            set { SetProperty(ref attachmentList, value); }
        }

        private ObservableCollection<FileInfo> selectedAttachments = new ObservableCollection<FileInfo>();

        public ObservableCollection<FileInfo> SelectedAttachments
        {
            get { return this.selectedAttachments; }
            set { SetProperty(ref selectedAttachments, value); }
        }

        private ObservableCollection<FileInfo> outlookSignatures = new ObservableCollection<FileInfo>();

        public ObservableCollection<FileInfo> OutlookSignatures

        {
            get { return this.outlookSignatures; }
            set { SetProperty(ref outlookSignatures, value); }
        }

        private FileInfo selectedSignature;

        public FileInfo SelectedSignature
        {
            get { return this.selectedSignature; }
            set { SetProperty(ref selectedSignature, value); }
        }

        public CheckLettersViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;           

            StartProcessesCommand = new DelegateCommand(OnStartProcesses);
            AddAttachmentCommand = new DelegateCommand(OnAddAttachment);
            RemoveAttachmentCommand = new DelegateCommand(OnRemoveAttachment);
            EmailBodyLoadedCommand = new DelegateCommand<object>(OnEmailBodyLoaded);
            EmailBodyContentChangedCommand = new DelegateCommand<object>(OnEmailBodyContentChanged);           
            NotificationRequest = new InteractionRequest<INotification>();

            // set Exportsettings for RichtextBox for E-Mail-Body

            HtmlExportSettings htmlExportSettings = new HtmlExportSettings();
            htmlExportSettings.DocumentExportLevel = DocumentExportLevel.Fragment;
            htmlExportSettings.StylesExportMode = StylesExportMode.Inline;
            htmlExportSettings.StyleRepositoryExportMode = StyleRepositoryExportMode.DontExportStyles;
            htmlExportSettings.ExportFontStylesAsTags = true;
            htmlExportSettings.ExportEmptyDocumentAsEmptyString = true;
            htmlFormatProvider.ExportSettings = htmlExportSettings;

            // Read OutlookAccounts
            GetOutlookAccounts();

            //Read OutlookSignatures
            ReadSignatures();

            string signaturFile = Properties.Settings.Default.EmailSignature;
            if (!string.IsNullOrEmpty(signaturFile))
            {
                foreach(FileInfo info in OutlookSignatures)
                {
                    if (info.FullName.Equals(signaturFile)) SelectedSignature = info;
                }
            }

            if (!string.IsNullOrEmpty(Properties.Settings.Default.EmailAccount))
            {
                string emailAddress = Properties.Settings.Default.EmailAccount;

               
                foreach(Account account in OutlookAccounts)
                {
                    if (account.DisplayName.Equals(emailAddress))
                    {
                        OutlookUsingAccount = account;
                        break;
                    }
                }
                
                if (OutlookUsingAccount != null) return;        //a valid outlookaccount exists               
            }
            // if no Account has been selected use the first Account
            if (OutlookAccounts.Count > 0) OutlookUsingAccount = OutlookAccounts[0];
            else
            {
                NotificationRequest.Raise(new Notification()
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = "Es existiert kein Outlook-Account für diesen User"
                });
            }           
        }

        private void OnEmailBodyContentChanged(object obj)
        {
            EmailBody = htmlFormatProvider.Export(HtmlBoxEmailBody.Document);
            if (CashFlowInformation.CashFlowType == "Capital Call")
            {
                Properties.Settings.Default.EmailBodyCall = EmailBody;
            }
            else
            {
                Properties.Settings.Default.EmailBodyDistribution = EmailBody;
            }
            Properties.Settings.Default.Save();
        }

        private void OnEmailBodyLoaded(object obj)
        {
            RoutedEventArgs e = obj as RoutedEventArgs;
            if (e == null) return;
            HtmlBoxEmailBody = e.Source as Telerik.Windows.Controls.RadRichTextBox;          
            HtmlBoxEmailBody.FontStyle = FontStyles.Normal;
            HtmlBoxEmailBody.FontWeight = FontWeights.Normal;
            HtmlBoxEmailBody.DocumentInheritsDefaultStyleSettings = true;
            if (CashFlowInformation.CashFlowType == "Capital Call")
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.EmailBodyCall))
                {
                    HtmlBoxEmailBody.Document = htmlFormatProvider.Import(Properties.Settings.Default.EmailBodyCall);
                    EmailBody = Properties.Settings.Default.EmailBodyCall;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.EmailBodyDistribution))
                {
                    HtmlBoxEmailBody.Document = htmlFormatProvider.Import(Properties.Settings.Default.EmailBodyDistribution);
                    EmailBody = Properties.Settings.Default.EmailBodyDistribution;
                }
            }
        }

 

     
 

        private void OnRemoveAttachment()
        {
            foreach(FileInfo info in SelectedAttachments)
            {
                AttachmentList.Remove(info);
            }
            SelectedAttachments.Clear();
        }

        private void OnAddAttachment()
        {
            Telerik.Windows.Controls.RadOpenFileDialog openFileDialog = new Telerik.Windows.Controls.RadOpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            openFileDialog.ShowDialog();
            if (openFileDialog.DialogResult != true) return;
            foreach(string name in openFileDialog.FileNames)
            {
                FileInfo info = new FileInfo(name);
                AttachmentList.Add(info);
               
            }
        }
  

        private void OnStartProcesses()
        {
           
            if (SaveCashFlowTransactions || PrepareEmails)
            {
                cashflowNumber = PefundAccess.GetUniqueCashFlowNumber(CashFlowInformation.EffectiveDate, CashFlowInformation.Fund.Id);
                
                try
                {
                    CreateClientLetterTelerik createLetters = new CreateClientLetterTelerik(CashFlowInformation, cashflowNumber, eventAggregator);
                    SaveCashFlows();
                }
                catch (System.Exception ex)
                {
                    NotificationRequest.Raise(new Notification()
                    {
                        Title = ApplicationNames.NotificationTitle,
                        Content = ex.Message
                    });
                }                
            }
            if (PrepareEmails)
            {
                GenerateEmails();
            }
          
        }

        private void SaveCashFlows()
        {
           
            List<InvestorCashFlow> cashFlows = new List<InvestorCashFlow>();
            foreach(CashFlowDetail detail in CashFlowInformation.InvestorDetails)
            {
                if (detail.CommitmentAmount == 0) continue;
                InvestorCashFlow cashFlow = new InvestorCashFlow();
                
                if (detail.CashFlowAmountCall != 0)
                {
                    // Capital Call erstellen
                    cashFlow.CashFlowAmount = Math.Abs(detail.CashFlowAmountCall) * -1;     //Amount for calls is negative
                    cashFlow.CashFlowDescription = CashFlowInformation.CashFlowDescription;
                    cashFlow.CashFlowNumber = cashflowNumber;
                    cashFlow.CashFlowType = "Capital Call";
                    cashFlow.CommitmentAmount = detail.CommitmentAmount;
                    cashFlow.EffectiveDate = CashFlowInformation.EffectiveDate;
                    cashFlow.InvestorCommitmentId = detail.InvestorCommitmentId;
                    cashFlow.LookbackInterests = Math.Abs(detail.LookbackInterests) * -1;     //Amount for calls is negative
                    cashFlow.PartnershipExpenses = Math.Abs(detail.PartnershipExpenses) * -1;     //Amount for calls is negative
                    cashFlow.UniqueCashFlowId = cashflowNumber;
                    cashFlows.Add(cashFlow);
                }
               
                if (detail.CashFlowAmountDistribution != 0)
                {
                    // Distribution erstellen
                    cashFlow.CashFlowAmount = Math.Abs(detail.CashFlowAmountDistribution);     //Amount for distributions are positive
                    cashFlow.CashFlowDescription = CashFlowInformation.CashFlowDescription;
                    cashFlow.CashFlowNumber = cashflowNumber;
                    cashFlow.CashFlowType = "Distribution";
                    cashFlow.CommitmentAmount = detail.CommitmentAmount;
                    cashFlow.EffectiveDate = CashFlowInformation.EffectiveDate;
                    cashFlow.InvestorCommitmentId = detail.InvestorCommitmentId;
                    cashFlow.LookbackInterests = Math.Abs(detail.LookbackInterests);     //Amount for distributions are positive
                    cashFlow.PartnershipExpenses = Math.Abs(detail.PartnershipExpenses);    //Sign is not changed
                    cashFlow.ReturnOfCapital = detail.ReturnOfCapital;
                    cashFlow.CapitalGain = detail.CapitalGain;
                    cashFlow.Dividends = detail.Dividends;
                    cashFlow.WithholdingTax = detail.WithholdingTax;
                    cashFlow.RecallableAmount = detail.RecallableAmount;
                    cashFlow.UniqueCashFlowId = cashflowNumber;
                    cashFlows.Add(cashFlow);
                }
            }
            // Add Cashflows to the database
            int numberOfAddedItems = 0;
            InvestorAccess access = new InvestorAccess();
            try
            {
                numberOfAddedItems = access.AddInvestorCashFlows(cashFlows);

                SaveCashFlowText = $"Es wurden {numberOfAddedItems.ToString()} Cashflows mit der Cashflow-Nummer {cashflowNumber.ToString()} in die Datenbank eingetragen";
                SaveCashFlowsDone = true;

            }
            catch (System.Exception ex)
            {
                NotificationRequest.Raise(new Notification()
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = ex.Message
                });
            }
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            CashFlowInformation = navigationContext.Parameters["Info"] as CashFlowInformation;

            if (CashFlowInformation.CashFlowType == "Capital Call")
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.EmailBodyCall))
                {
                    HtmlBoxEmailBody.Document = htmlFormatProvider.Import(Properties.Settings.Default.EmailBodyCall);
                    EmailBody = Properties.Settings.Default.EmailBodyCall;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Properties.Settings.Default.EmailBodyDistribution))
                {
                    HtmlBoxEmailBody.Document = htmlFormatProvider.Import(Properties.Settings.Default.EmailBodyDistribution);
                    EmailBody = Properties.Settings.Default.EmailBodyDistribution;
                }
            }
        }

        private void GenerateEmails()
        {

            // emails can only be sent if there is a valid outlookAccount

            if (outlookUsingAccount == null) return;
            foreach (CashFlowDetail detail in CashFlowInformation.InvestorDetails)
            {

                if (detail.Investor.SendEmail == false) continue;

                MailItem mailItem = outlookApp.CreateItem(OlItemType.olMailItem);

                mailItem.BodyFormat = OlBodyFormat.olFormatHTML;
                mailItem.SendUsingAccount = outlookUsingAccount;
                mailItem.Subject = CashFlowInformation.CashFlowDescription;
                Recipient recipTo = mailItem.Recipients.Add(detail.Investor.ClientAdvisor.AdvisorName.EmailAddress);
                recipTo.Type = (int)OlMailRecipientType.olTo;

                if (!mailItem.Recipients.ResolveAll())
                {
                    // Fehler bei den E-Mail-Adressen
                }

                StringBuilder bodyText = new StringBuilder(EmailBody);               
                bodyText.Replace("[Anrede]", detail.Investor.ClientAdvisor.AdvisorName.Salutation);
                bodyText.Replace("[Fondsname]", CashFlowInformation.Fund.FundLegalName);
                bodyText.Replace("[Betrag]", $"{Math.Abs(detail.CashFlowAmount):n2} {CashFlowInformation.Fund.Currency.CurrencyShortName}");
                bodyText.Replace("[Faelligkeit]", $"{CashFlowInformation.EffectiveDate:d}");

                mailItem.HTMLBody = bodyText.ToString();  

                mailItem.Attachments.Add(detail.FileName, OlAttachmentType.olByValue);

                // add further Attachments 

                foreach(FileInfo info in AttachmentList)
                {
                    mailItem.Attachments.Add(info.FullName, OlAttachmentType.olByValue);
                }

                //AddSignature

                if(SelectedSignature!= null)
                {
                    // save selectedSignatur as property
                    Properties.Settings.Default.EmailSignature = SelectedSignature.FullName;
                    Properties.Settings.Default.Save();

                    string signature = GetSignature(SelectedSignature.FullName);
                    if (signature.Contains("img"))
                    {
                        int position = signature.LastIndexOf("img");
                        int position1 = signature.IndexOf("src", position);
                        position1 = position1 + 5;
                        position = signature.IndexOf("\"", position1);

                        //CONTENT-ID
                        const string SchemaPR_ATTACH_CONTENT_ID = @"http://schemas.microsoft.com/mapi/proptag/0x3712001E";
                        string contentID = Guid.NewGuid().ToString();

                        //Attach image
                        mailItem.Attachments.Add(@billede1, Microsoft.Office.Interop.Outlook.OlAttachmentType.olByValue, mailItem.HTMLBody.Length, Type.Missing);
                        mailItem.Attachments[mailItem.Attachments.Count].PropertyAccessor.SetProperty(SchemaPR_ATTACH_CONTENT_ID, contentID);

                        //Create and add banner
                        string banner = string.Format(@"cid:{0}", contentID);
                        signature = signature.Remove(position1, position - position1);
                        signature = signature.Insert(position1, banner);

                        position = signature.LastIndexOf("imagedata");
                        position1 = signature.IndexOf("src", position);
                        position1 = position1 + 5;
                        position = signature.IndexOf("\"", position1);
                        
                        contentID = Guid.NewGuid().ToString();

                        //Attach image
                        mailItem.Attachments.Add(@billede2, OlAttachmentType.olByValue, mailItem.HTMLBody.Length, Type.Missing);
                        mailItem.Attachments[mailItem.Attachments.Count].PropertyAccessor.SetProperty(SchemaPR_ATTACH_CONTENT_ID, contentID);

                        //Create and add banner
                        banner = string.Format(@"cid:{0}", contentID);
                        signature = signature.Remove(position1, position - position1);
                        signature = signature.Insert(position1, banner);
                    }
                    mailItem.HTMLBody = mailItem.HTMLBody + signature;
                }

                mailItem.Save();

                // if E-Mails should be sent to more than one person:


            }
        }

        private string GetSignature(string fullName)
        {
            string signature = string.Empty;

            using (StreamReader sr = new StreamReader(fullName, Encoding.Default))
            {
                signature = sr.ReadToEnd();
            }               
            
            if (signature.Contains("img"))
            {
                int position = signature.LastIndexOf("img");
                int position1 = signature.IndexOf("src", position);
                position1 = position1 + 5;
                position = signature.IndexOf("\"", position1);
                billede1 = appDataDir.ToString() + "\\" + signature.Substring(position1, position - position1);
                position = billede1.IndexOf("/");
                billede1 = billede1.Remove(position, 1);
                billede1 = billede1.Insert(position, "\\");

                billede1 = System.Web.HttpUtility.UrlDecode(billede1);


                position = signature.LastIndexOf("imagedata");
                position1 = signature.IndexOf("src", position);
                position1 = position1 + 5;
                position = signature.IndexOf("\"", position1);
                billede2 = appDataDir.ToString() + "\\" + signature.Substring(position1, position - position1);
                position = billede2.IndexOf("/");
                billede2 = billede2.Remove(position, 1);
                billede2 = billede2.Insert(position, "\\");

                billede2 = System.Web.HttpUtility.UrlDecode(billede2);
            }
            return signature;
        }
        private void GetOutlookAccounts()
        {
            NameSpace ns = null;
            Accounts accounts = null;

            OutlookAccounts.Clear();
            try
            {
                ns = outlookApp.Session;
                accounts = ns.Accounts;

                for (int i = 1; i <= accounts.Count; i++)
                {
                    OutlookAccounts.Add(accounts[i]);   
                }
            }
            catch (System.Exception ex)
            {
                NotificationRequest.Raise(new Notification()
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = ex.Message
                });
            }

        }

  
        private void ReadSignatures()
        {
            appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Signatures";
           
            DirectoryInfo diInfo = new DirectoryInfo(appDataDir);

            if (diInfo.Exists)
            {
                OutlookSignatures = new ObservableCollection<FileInfo>( diInfo.GetFiles("*.htm"));  
            }  
        }
    }
}
