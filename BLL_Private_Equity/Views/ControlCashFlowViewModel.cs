using BLL_DataAccess;
using BLL_DataModels;
using BLL_Private_Equity.Berechnungen;
using BLL_Private_Equity.Events;
using BLL_Infrastructure;
using BLL_Prism;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BLL_Private_Equity.Views
{
    public class ControlCashFlowViewModel: HqtBindableBase
    {

        private PeFund   fund;

        public PeFund Fund
        {
            get { return this.fund; }
            set { SetProperty(ref fund, value); }
        }

        private bool checkClientDataDone;

        public bool CheckClientDataDone
        {
            get { return this.checkClientDataDone; }
            set { SetProperty(ref checkClientDataDone, value); }
        }


        private bool checkCashFlowDataDone;

        public bool CheckCashFlowDataDone
        {
            get { return this.checkCashFlowDataDone; }
            set { SetProperty(ref checkCashFlowDataDone, value); }
        }

        private bool checkInvestorSplitDone;

        public bool CheckInvestorSplitDone
        {
            get { return this.checkInvestorSplitDone; }
            set { SetProperty(ref checkInvestorSplitDone, value); }
        }

        private bool checkLettersDone;

        public bool CheckLettersDone
        {
            get { return this.checkLettersDone; }
            set { SetProperty(ref checkLettersDone, value); }
        }

        private bool checkOtherWorkDone;
       

        public bool CheckOtherWorkDone
        {
            get { return this.checkOtherWorkDone; }
            set { SetProperty(ref checkOtherWorkDone, value); }
        }

        private bool? newCashFlow;
        private FileInfo textFileInfo;
        private CashFlowInformation cashFlowInformation = new CashFlowInformation();
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;

        public ICommand CheckInvestorDataCommand { get; set; }
        public ICommand CheckCashFlowDataCommand { get; set; }
        public ICommand CheckInvestorSplitCommand { get; set; }
        public ICommand CheckLettersCommand { get; set; }
        public ICommand CheckOtherWorkCommand { get; set; }

        public InteractionRequest<INotification> NotificationRequest { get; set; }
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }

        public ControlCashFlowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;

            CheckInvestorDataCommand = new DelegateCommand(OnCheckInvestorData);
            CheckCashFlowDataCommand = new DelegateCommand(OnCheckCashFlowData).ObservesCanExecute(() => CheckClientDataDone);
            CheckInvestorSplitCommand = new DelegateCommand(OnCheckInvestorSplit).ObservesCanExecute(() => CheckCashFlowDataDone);
            CheckLettersCommand = new DelegateCommand(OnCheckLetters).ObservesCanExecute(() => CheckInvestorSplitDone);
            CheckOtherWorkCommand = new DelegateCommand(OnCheckOtherWork).ObservesCanExecute(() => CheckLettersDone);

            NotificationRequest = new InteractionRequest<INotification>();
            ConfirmationRequest = new InteractionRequest<IConfirmation>();
            eventAggregator.GetEvent<PrepareCashFlowEvent>().Subscribe(OnPrepareCashFlow);
        }

        private void OnPrepareCashFlow(PrepareCashFlow obj)
        {
            cashFlowInformation = obj.cfInfo;
            WriteInformationToTextFile();
            SetProgressStatus();
            // the next step will be automatically started 

            if (cashFlowInformation.OtherWorkDone) return;  //CashFlow has been processed; no further action required
            if (cashFlowInformation.LettersPrinted)
            {
                // start OtherWork
                return;
            }
            if (cashFlowInformation.InvestorDetailsEntered)
            {
                // start Print Letters
                NavigationParameters parameter = new NavigationParameters();
                parameter.Add("Info", cashFlowInformation);
                regionManager.RequestNavigate(RegionNames.CashFlowRegion, ViewNames.CheckLetters, parameter);
                return;                
            }
            if (cashFlowInformation.CashFlowDataEntered)
                {
                // start Enter CashFlowData for investors
                NavigationParameters parameter = new NavigationParameters();
                parameter.Add("Info", cashFlowInformation);
                regionManager.RequestNavigate(RegionNames.CashFlowRegion, ViewNames.EditInvestorSplit, parameter);
                return;
            }
            if (cashFlowInformation.InvestorsChecked)
            {
                // start Enter CashFlowData
                NavigationParameters parameter = new NavigationParameters();
                parameter.Add("Info", cashFlowInformation);
                regionManager.RequestNavigate(RegionNames.CashFlowRegion, ViewNames.EditCashFlowData, parameter);
                return;
            }
        }

        private void OnCheckOtherWork()
        {
            throw new NotImplementedException();
        }

        private void OnCheckLetters()
        {
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Info", cashFlowInformation);
            regionManager.RequestNavigate(RegionNames.CashFlowRegion, ViewNames.CheckLetters, parameter);
            return;
        }

        private void OnCheckInvestorSplit()
        {
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Info", cashFlowInformation);
            regionManager.RequestNavigate(RegionNames.CashFlowRegion, ViewNames.EditInvestorSplit, parameter);
            return;
        }

        private void OnCheckCashFlowData()
        {
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Info", cashFlowInformation);
            regionManager.RequestNavigate(RegionNames.CashFlowRegion, ViewNames.EditCashFlowData, parameter);
            return;
        }

        private void OnCheckInvestorData()
        {
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Info", cashFlowInformation);
            regionManager.RequestNavigate(RegionNames.CashFlowRegion, ViewNames.CheckInvestorData, parameter);
            return;
        }
        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Fund = navigationContext.Parameters["Fund"] as PeFund;
            newCashFlow = (bool?)navigationContext.Parameters["Status"];
            if (newCashFlow == null) return;

            TabTitle = "Cash Flow";

            if (GetFileData(Fund) == false)
            {
                cashFlowInformation = new CashFlowInformation();
                CreateCashFlowInformation(cashFlowInformation);                
                return;
            }

            if ((bool)newCashFlow)
            {
                // get confirmation that existing cashflowinformation was finalized
                // if not confirmed show existing cashflowinformation
                // if confirmed set new confirmation and write info to disk
                //          and return function
                if (cashFlowInformation.OtherWorkDone==false)
                {
                    ConfirmationRequest.Raise(new Confirmation()
                    {
                        Title = ApplicationNames.NotificationTitle,
                        Content = "Der existierende CashFlow wurde noch nicht abgeschlossen. Wollen Sie einen neuen CashFlow erfassen? "
                    }, OnConfirmNewCashFlow);
                }
                cashFlowInformation = new CashFlowInformation();
                CreateCashFlowInformation(cashFlowInformation);
                cashFlowInformation.FileName = textFileInfo.FullName;
                return;
            }
            else
            {
                SetProgressStatus();
                cashFlowInformation.FileName = textFileInfo.FullName;

                if (cashFlowInformation.OtherWorkDone) return;  //CashFlow has been processed; no further action required
                if (cashFlowInformation.LettersPrinted)
                {
                    // start OtherWork
                    return;
                }
                if (cashFlowInformation.InvestorDetailsEntered)
                {
                    // start Print Letters
                    NavigationParameters parameter = new NavigationParameters();
                    parameter.Add("Info", cashFlowInformation);
                    regionManager.RequestNavigate(RegionNames.CashFlowRegion, ViewNames.CheckLetters, parameter);
                    return;                   
                }
                if (cashFlowInformation.CashFlowDataEntered)
                {
                    // start Enter CashFlowData for investors
                    NavigationParameters parameter = new NavigationParameters();
                    parameter.Add("Info", cashFlowInformation);
                    regionManager.RequestNavigate(RegionNames.CashFlowRegion, ViewNames.EditInvestorSplit, parameter);
                    return;
                }
                if (cashFlowInformation.InvestorsChecked)
                {
                    // start Enter CashFlowData
                    NavigationParameters parameter = new NavigationParameters();
                    parameter.Add("Info", cashFlowInformation);
                    regionManager.RequestNavigate(RegionNames.CashFlowRegion, ViewNames.EditCashFlowData, parameter);
                    return;
                }
                // start check InvestorData
                NavigationParameters parameter1 = new NavigationParameters();
                parameter1.Add("Info", cashFlowInformation);
                regionManager.RequestNavigate(RegionNames.CashFlowRegion, ViewNames.CheckInvestorData, parameter1);
                return;
                return;
            }
            // set status
                      
        }

        private void SetProgressStatus()
        {
            CheckClientDataDone = cashFlowInformation.InvestorsChecked;
            CheckCashFlowDataDone = cashFlowInformation.CashFlowDataEntered;
            CheckInvestorSplitDone = cashFlowInformation.InvestorDetailsEntered;
            CheckLettersDone = cashFlowInformation.LettersPrinted;
            CheckOtherWorkDone = cashFlowInformation.OtherWorkDone;           
        }

        private void OnConfirmNewCashFlow(IConfirmation obj)
        {
            if (obj.Confirmed)
            {
                cashFlowInformation = new CashFlowInformation();
                CreateCashFlowInformation(cashFlowInformation);
            }
        }

        private void CreateCashFlowInformation(CashFlowInformation info)
        {
            info.Fund = Fund;
            info.LastUpdated = DateTime.Now.Date;
            if (info.EffectiveDate == DateTime.MinValue) info.EffectiveDate = DateTime.Now.Date;
            List<InvestorCommitment> ListOfCommitments = PefundAccess.GetCommitmentsForPeFund(Fund.Id);
            CashFlowDetail totalDetail = new CashFlowDetail()
            {               
                Investor = null
            };

            if (Fund.BankAccounts.Count == 0) totalDetail.BankAccount = null;
            else totalDetail.BankAccount = Fund.BankAccounts.ElementAt(0);

            info.InvestorDetails = new List<CashFlowDetail>();

            foreach (InvestorCommitment commitment in ListOfCommitments)
            {
                PeFundResults results = new PeFundResults(commitment, DateTime.MinValue, DateTime.Now);

                CashFlowDetail investorDetail = new CashFlowDetail()
                {
                    Investor = commitment.Investor,
                    Reference = commitment.Investor.InvestorReference,
                    BankAccount = commitment.BankAccount,
                    CommitmentAmount = commitment.CommitmentAmount,
                    OpenCommitment = results.OpenCommitment,
                    TotalCalls = results.AmountCalled,
                    TotalDistributions = results.AmountDistributed, 
                    InvestorCommitmentId = commitment.Id                    
                };
                if (!string.IsNullOrEmpty(commitment.PeFundReference)) investorDetail.Reference = commitment.PeFundReference;
                totalDetail.CommitmentAmount += commitment.CommitmentAmount;
                info.InvestorDetails.Add(investorDetail);
            }
            info.DetailSummary = totalDetail;
            WriteInformationToTextFile();
            SetProgressStatus();
            

            if (info.OtherWorkDone) return;  //CashFlow has been processed; no further action required
            if (info.LettersPrinted)
            {
                // start OtherWork
                return;
            }
            if (info.InvestorDetailsEntered)
            {
                // start Print Letters
                return;
            }
            if (info.CashFlowDataEntered)
            {
                // start Enter CashFlowData for investors
                NavigationParameters parameter = new NavigationParameters();
                parameter.Add("Info", info);
                regionManager.RequestNavigate(RegionNames.CashFlowRegion, ViewNames.EditInvestorSplit, parameter);
                return;
            }
            if (info.InvestorsChecked)
            {
                // start Enter CashFlowData
                NavigationParameters parameter = new NavigationParameters();
                parameter.Add("Info", info);
                regionManager.RequestNavigate(RegionNames.CashFlowRegion, ViewNames.EditCashFlowData, parameter);
                return;
            }
        }

        private bool GetFileData(PeFund peFund)
        {
            DirectoryHelper.CheckDirectory($"TextFiles");
            textFileInfo = DirectoryHelper.GetTextFileName(Fund.Id);
            if (textFileInfo.Exists)
            {

                string fileContent = string.Empty;

                using (StreamReader sr = new StreamReader(textFileInfo.FullName))
                {
                    fileContent = sr.ReadToEnd();
                }


                if (!string.IsNullOrEmpty(fileContent)) cashFlowInformation = JsonConvert.DeserializeObject<CashFlowInformation>(fileContent);

                if (cashFlowInformation == null)
                {
                    NotificationRequest.Raise(new Notification()
                    {
                        Title = ApplicationNames.NotificationTitle,
                        Content = "Die Ausschüttungsdaten konnten nicht gelesen werden"
                    });
                    return false;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// writes the cashflowinformation to disk
        /// </summary>
        private void WriteInformationToTextFile()
        {
            //string output = JsonConvert.SerializeObject(cashFlowInformation);
            
            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            using (StreamWriter sw = new StreamWriter(textFileInfo.FullName))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, cashFlowInformation);
                }
            }
        }

        private void MoveToCheckInvestorData()
        {
            NavigationParameters parameter = new NavigationParameters();
            parameter.Add("Info", cashFlowInformation);
            regionManager.RequestNavigate(RegionNames.CashFlowRegion, ViewNames.CheckInvestorData, parameter);
        }
    }
}
