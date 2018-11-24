using BLL_DataModels;
using BLL_Private_Equity.Events;
using BLL_Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace BLL_Private_Equity.Views
{
    public class EditInvestorCommitmentViewModel : HqtBindableBase
    {
        private InvestorCommitment copyOfInvestorCommitment;
        public ObservableCollection<PeFund> AvailableFunds { get; set; }
        public ObservableCollection<BankAccount> BankAccounts { get; set; }
        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand AddCommitmentDetailCommand { get; set; }
        public ICommand DeletedCommitmentDetailCommand { get; set; }

        private InvestorCommitmentDetail selectedInvestorCommitmentDetail;

        public InvestorCommitmentDetail SelectedInvestorCommitmentDetail
        {
            get { return this.selectedInvestorCommitmentDetail; }
            set { SetProperty(ref selectedInvestorCommitmentDetail, value); }
        }


        private ObservableCollection<InvestorCommitmentDetail> investorCommitmentDetails;

        public ObservableCollection<InvestorCommitmentDetail> InvestorCommitmentDetails
        {
            get { return this.investorCommitmentDetails; }
            set { SetProperty(ref investorCommitmentDetails, value); }
        }

        private InvestorCommitment investorCommitment;

        public InvestorCommitment InvestorCommitment
        {
            get { return this.investorCommitment; }
            set { SetProperty(ref investorCommitment, value); }
        }
        private readonly IEventAggregator eventAggregator;

        public EditInvestorCommitmentViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            eventAggregator.GetEvent<InvokeEditInvestorCommitmentEvent>().Subscribe(OnInvokeEditInvestorCommitment);
            OkCommand = new DelegateCommand(OnOKButton);
            CancelCommand = new DelegateCommand(OnCancelButton);
            AddCommitmentDetailCommand = new DelegateCommand(OnAddCommitmentDetail);
            DeletedCommitmentDetailCommand = new DelegateCommand(OnDeletedCommitmentDetail);
        }

        private void OnDeletedCommitmentDetail()
        {
            InvestorCommitment.CommitmentAmount = 0;
            foreach (InvestorCommitmentDetail detail in InvestorCommitmentDetails)
            {
                InvestorCommitment.CommitmentAmount += detail.CommitmentAmount;
                if (InvestorCommitment.DateCommitmentAccepted == null) InvestorCommitment.DateCommitmentAccepted = detail.CommitmentDate;
            }
        }

        private void OnAddCommitmentDetail()
        {
            SelectedInvestorCommitmentDetail.PropertyChanged -= SelectedInvestorCommitmentDetail_PropertyChanged;
            InvestorCommitmentDetail detail = new InvestorCommitmentDetail()
            {
                InvestorCommitmentId = InvestorCommitment.Id,
                CommitmentDate = DateTime.Now
            };
            InvestorCommitmentDetails.Add(detail);
            SelectedInvestorCommitmentDetail = InvestorCommitmentDetails[InvestorCommitmentDetails.Count - 1];
            SelectedInvestorCommitmentDetail.PropertyChanged += SelectedInvestorCommitmentDetail_PropertyChanged;
        }

        private void SelectedInvestorCommitmentDetail_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CommitmentAmount")
            {
                // update commitmentamount of InvestorCommitment
                InvestorCommitment.CommitmentAmount = 0;
                foreach (InvestorCommitmentDetail detail in InvestorCommitmentDetails)
                {
                    InvestorCommitment.CommitmentAmount += detail.CommitmentAmount;
                    if (InvestorCommitment.DateCommitmentAccepted == null) InvestorCommitment.DateCommitmentAccepted = detail.CommitmentDate;                   
                }
            }
        }

        private void OnCancelButton()
        {
            InvestorCommitment = new InvestorCommitment(copyOfInvestorCommitment);
            EditInvestorCommitmentResponse response = new EditInvestorCommitmentResponse()
            {
                ActionType = "Cancel",
                editedCommitment = null
            };
            eventAggregator.GetEvent<EditInvestorCommitmentResponseEvent>().Publish(response);
        }

        private void OnOKButton()
        {
            InvestorCommitment.InvestorCommitmentDetails = new System.Collections.Generic.List<InvestorCommitmentDetail>();
            foreach (InvestorCommitmentDetail detail in InvestorCommitmentDetails)
            {
                InvestorCommitment.InvestorCommitmentDetails.Add(detail);
            }
            
            EditInvestorCommitmentResponse response = new EditInvestorCommitmentResponse()
            {
                ActionType = "OK",
                editedCommitment = InvestorCommitment
            };
            eventAggregator.GetEvent<EditInvestorCommitmentResponseEvent>().Publish(response);
        }

        private void OnInvokeEditInvestorCommitment(InvokeEditInvestorCommitment parameter)
        {
            if (InvestorCommitment != null) InvestorCommitment.PropertyChanged -= InvestorCommitment_PropertyChanged;
            InvestorCommitment = parameter.investorCommitment;
            InvestorCommitment.PropertyChanged += InvestorCommitment_PropertyChanged;

            // create a copy
            // will be used when user cancels input
            copyOfInvestorCommitment = new InvestorCommitment(InvestorCommitment);
            AvailableFunds = new ObservableCollection<PeFund>();
            AvailableFunds = parameter.availableFunds;
            BankAccounts = parameter.bankAccounts;
            InvestorCommitmentDetails = new ObservableCollection<InvestorCommitmentDetail>(InvestorCommitment.InvestorCommitmentDetails);
            // Select first record if available
            if (InvestorCommitmentDetails.Count == 0)
            {
                InvestorCommitmentDetails.Add(new InvestorCommitmentDetail()
                {
                    CommitmentDate=DateTime.Now
                });
            }
            SelectedInvestorCommitmentDetail = InvestorCommitmentDetails[0];
            SelectedInvestorCommitmentDetail.PropertyChanged += SelectedInvestorCommitmentDetail_PropertyChanged;
            AddDisplayNameToBankAccounts();
            RaisePropertyChanged("AvailableFunds");
            RaisePropertyChanged("InvestorCommitment");
            RaisePropertyChanged("BankAccounts");
        }

        private void InvestorCommitment_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           if (e.PropertyName == "PeFundId")
            {
                PeFund fund = AvailableFunds.FirstOrDefault(f => f.Id == InvestorCommitment.PeFundId);
                if (fund!= null)
                {
                    InvestorCommitment.PeFund = fund;
                    RaisePropertyChanged("PeFund");
                }
            }
        }

        private void AddDisplayNameToBankAccounts()
        {
            foreach (BankAccount account in BankAccounts)
            {
                if (string.IsNullOrEmpty(account.Iban))
                {
                    account.DisplayName = $"{account.AccountNumber} ({account.CurrencyName}) bei {account.BankName}";
                }
                else
                {
                    account.DisplayName = $"{account.Iban} ({account.CurrencyName}) bei {account.BankName}";
                }
            }
        }
    }
}
