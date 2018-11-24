using BLL_DataModels;
using BLL_Prism;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using DateTimeFunctions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Data;
using System.ComponentModel;
using BLL_DataAccess;
using System.Windows.Input;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using BLL_Infrastructure;
using System.Collections.ObjectModel;

namespace BLL_Private_Equity.Views
{
    public class EditPcapsViewModel: HqtBindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        DateDifferences dateFunctions = new DateDifferences();
        private DateTime selectedQuarter = DateTime.MinValue;
        private List<InvestorCommitment> commitments = null;
        private double totalCommitments = 0;
        private bool isNavCalculating = false;


        private double newPcap;

        public double NewPcap
        {
            get { return this.newPcap; }
            set { SetProperty(ref newPcap, value); }
        }

        private double pcapDifference;

        public double PcapDifference
        {
            get { return this.pcapDifference; }
            set { SetProperty(ref pcapDifference, value); }
        }

        private bool containsCalculatedItems = false;

        public bool ContainsCalculatedItems
        {
            get { return this.containsCalculatedItems; }
            set { SetProperty(ref containsCalculatedItems, value); }
        }

        private PeFund   fund = null;

        public PeFund Fund
        {
            get { return this.fund; }
            set { SetProperty(ref fund, value); }
        }
        private List<DateTime> quarters  = new List<DateTime>();
        public ICollectionView Quarters { get; set; }

        private ObservableCollection<InvestorPcap> pcaps = new ObservableCollection<InvestorPcap>();

        public ICollectionView Pcaps { get; set; }
        public ICommand AddNewPcapsCommand { get; set; }
        public ICommand OnValuationChangedCommand { get; set; }
        public ICommand RowEditEndedCommand { get; set; }
        public ICommand SaveChangesCommand { get; set; }
        public ICommand ExportToExcelCommand { get; set; }
        public InteractionRequest<INotification> NotificationRequest { get; set; }
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }

        public EditPcapsViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            AddNewPcapsCommand = new DelegateCommand(OnAddNewPcaps);
            OnValuationChangedCommand = new DelegateCommand(OnValuationChanged);
            SaveChangesCommand = new DelegateCommand(OnSaveChanges);
            ExportToExcelCommand = new DelegateCommand(OnExportToExcel);
            RowEditEndedCommand = new DelegateCommand(OnRowEditEnded);
            NotificationRequest = new InteractionRequest<INotification>();
            ConfirmationRequest = new InteractionRequest<IConfirmation>();

            FillQuarters();
        }

        private void OnExportToExcel()
        {
            throw new NotImplementedException();
        }

        private void OnSaveChanges()
        {
            try
            {
                PefundAccess.UpdateOrInsertPcaps(new List<InvestorPcap>(pcaps));
            }
            catch (Exception ex)
            {
                NotificationRequest.Raise(new Notification()
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = $"Beim Ändern der Datenbank ist ein Fehler aufgetreten. {ex.Message}"
                });
            }
        }

        private void OnRowEditEnded()
        {           
            // calculate sum of Pcaps and compare with NewPcap
            // display difference if any
            PcapDifference = NewPcap;
            foreach (InvestorPcap pcap in pcaps)
            {
                PcapDifference -= Math.Round(pcap.FinalPcapAmount,2);
            }
            PcapDifference = Math.Round(PcapDifference, 2);
        }

        private void OnValuationChanged()
        {
            if (isNavCalculating) return;
            PcapDifference = Math.Round(NewPcap,2);
            foreach (InvestorPcap pcap in pcaps)
            {
                pcap.FinalPcapAmount = Math.Round(NewPcap / totalCommitments * pcap.CommitmentAmount, 2);
                PcapDifference -= pcap.FinalPcapAmount;
            }
            //PcapDifference can be != 0 due to rounding
            // adjust first entry and set Difference to 0
            pcaps.ElementAt(0).FinalPcapAmount += PcapDifference;
            PcapDifference = 0;
        }

        private void OnAddNewPcaps()
        {
            
        }

        private void FillQuarters()
        {
            // generating a list of quarters descending from current date
           
            DateTime quarter = DateTime.Now.Date;
            while (quarter>new DateTime(1995,1,1))
            {
                quarter = dateFunctions.PreviousQuarter(quarter);
                quarters.Add(quarter.Date);
            }
            Quarters = CollectionViewSource.GetDefaultView(quarters);
            Quarters.CurrentChanged += Quarters_CurrentChanged;
            Quarters.MoveCurrentToFirst();
        }

        private void Quarters_CurrentChanged(object sender, EventArgs e)
        {
            // read Pcaps for the selected quarter

            selectedQuarter = (DateTime)Quarters.CurrentItem;

            if (commitments == null) return; 
            CreatePcapList();    
            if (pcaps.Count ==0)
            {
                ConfirmationRequest.Raise(new Confirmation()
                {
                    Title = ApplicationNames.NotificationTitle,
                    Content = "für dieses Quartalsende liegen keine Bewertungen vor." + Environment.NewLine +
                    "Sollen neue Bewertungen hinzugefügt werden?"
                }, OnConfirmNewPcaps);
            }
        }

        private void OnConfirmNewPcaps(IConfirmation obj)
        {
           
           if(obj.Confirmed)
            {
                isNavCalculating = true;
                NewPcap = 0;
                pcaps.Clear();
                // neue Pcaps anlegen (errechnen aus letztem Pcap + cashflows)
                foreach (InvestorCommitment c in commitments)
                {
                    InvestorPcap pcap = new InvestorPcap()
                    {
                        AsOfDate=selectedQuarter.Date,
                        DateOfFinalPcap = DateTime.Now,
                        FinalPcapAmount=0,
                        EstimatedPcapAmount=0,
                        FinalPcapAmountInInvestorCurrency=0,                        
                        InvestorCommitmentId=c.Id,
                        InvestorHqTrustAccount=c.Investor.InvestorHqTrustAccount,
                        InvestorReference=c.Investor.InvestorReference,
                        CommitmentAmount=c.CommitmentAmount
                    };
                    pcap.EstimatedPcapAmount = CalculatePcap(c, selectedQuarter);
                    pcap.FinalPcapAmount = pcap.EstimatedPcapAmount;
                    NewPcap += pcap.EstimatedPcapAmount;
                    pcaps.Add(pcap);
                }
                isNavCalculating = false;
            }
            RaisePropertyChanged("Pcaps");
        }

        private double CalculatePcap(InvestorCommitment c, DateTime selectedQuarter)
        {
            InvestorPcap lastPcap = PefundAccess.GetLastPCap(c, selectedQuarter);
            if (lastPcap == null)
            {
                lastPcap = new InvestorPcap()
                {
                    InvestorCommitmentId = c.Id,
                    AsOfDate = DateTime.MinValue,
                    FinalPcapAmount = 0
                };
            }
            return PefundAccess.NavCalculation(lastPcap, selectedQuarter);
        }

        private void CreatePcapList()
        {
            pcaps.Clear();
            isNavCalculating = true;
            NewPcap = 0;
            totalCommitments = 0;
            ContainsCalculatedItems = false;
            foreach (InvestorCommitment c in commitments)
            {
                InvestorPcap pcap = PefundAccess.GetPcapForCommitmentAndDate(c.Id, selectedQuarter);
                if (pcap == null)
                {
                    // if there is no record --> create a new record
                    // this record may be inserted into the database when saving changes
                    pcap = new InvestorPcap()
                    {
                        AsOfDate = selectedQuarter,
                        DateOfFinalPcap = DateTime.Now,
                        InvestorCommitmentId = c.Id
                    };
                    pcap.EstimatedPcapAmount = CalculatePcap(c, selectedQuarter);
                    pcap.FinalPcapAmount = pcap.EstimatedPcapAmount;
                    pcap.IsCalculated = true;
                    ContainsCalculatedItems = true;
                }

                // Add InvestorReference and InvestorHqTrustAccount to display properties in the list

                pcap.InvestorReference = c.Investor.InvestorReference;
                pcap.InvestorHqTrustAccount = c.Investor.InvestorHqTrustAccount;
                pcap.CommitmentAmount = c.CommitmentAmount;
                pcaps.Add(pcap);
                NewPcap += Math.Round(pcap.FinalPcapAmount,2);
                totalCommitments += c.CommitmentAmount;
            }
            //if (Pcaps != null) Pcaps.CurrentChanged -= Pcaps_CurrentChanged;
            Pcaps = CollectionViewSource.GetDefaultView(pcaps);
            //Pcaps.CurrentChanged += Pcaps_CurrentChanged;
            Pcaps.MoveCurrentToFirst();
            RaisePropertyChanged("Pcaps");
            if (Pcaps.CurrentItem == null) return;
            // calculate sum of Pcaps and compare with NewPcap
            // display difference if any
            PcapDifference = NewPcap;
            foreach (InvestorPcap pcap in pcaps)
            {
                PcapDifference -= Math.Round(pcap.FinalPcapAmount,2);
            }
            PcapDifference = Math.Round(PcapDifference, 2);
            isNavCalculating = false;
        }

        private void Pcaps_CurrentChanged(object sender, EventArgs e)
        {
 
        }
        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //if (Pcaps != null) Pcaps.CurrentChanged -= Pcaps_CurrentChanged;
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            pcaps = new ObservableCollection<InvestorPcap>();
            Fund = navigationContext.Parameters["Fund"] as PeFund;
            commitments = PefundAccess.GetCommitmentsForPeFund(Fund.Id);
            if (selectedQuarter != DateTime.MinValue) CreatePcapList();
            Pcaps = CollectionViewSource.GetDefaultView(pcaps);
            Pcaps.CurrentChanged += Pcaps_CurrentChanged;

            // set tabtitle

            if (string.IsNullOrEmpty(Fund.FundHqTrustNumber))
                TabTitle = Fund.FundName + " (NAVs)";
            else TabTitle = Fund.FundHqTrustNumber + " (NAVs)";
        }
        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            PeFund myFund = navigationContext.Parameters["Fund"] as PeFund;
            if (Fund == null) return true;
            if (Fund.Id == myFund.Id) return true;
            return false;
        }
    }
}
