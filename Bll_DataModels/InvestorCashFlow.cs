using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_DataModels
{
    public class InvestorCashFlow: INotifyPropertyChanged
    {
        private int id;
        private long psPlusId;
        private int investorCommitmentId;
        private int? uniqueCashFlowId;
        private string cashFlowType;
        private int cashFlowNumber;
        private DateTime effectiveDate;
        private string cashFlowDescription;
        private double commitmentAmount;
        private double cashFlowAmount;
        private double cashFlowAmountInInvestorCurrency;
        private double returnOfCapital;
        private double capitalGain;
        private double openPayments;
        private double dividends;
        private double otherIncome;
        private double recallableAmount;
        private double partnershipExpenses;
        private double withholdingTax;
        private double investorExpenses;
        private double lookbackInterests;

        [JsonIgnore]
        public virtual InvestorCommitment InvestorCommitment { get; set; }

        public long PsPlusId
        {
            get { return psPlusId; }
            set { psPlusId = value; OnPropertyChanged("PsPlusId"); }
        }

        public double CashFlowAmountInInvestorCurrency
        {
            get
            {
                return this.cashFlowAmountInInvestorCurrency;
            }
            set
            {
                this.cashFlowAmountInInvestorCurrency = value;
                OnPropertyChanged("CashFlowAmountInInvestorCurrency");
            }
        }

        public int InvestorCommitmentId
        {
            get
            {
                return this.investorCommitmentId;
            }
            set
            {
                this.investorCommitmentId = value;
                OnPropertyChanged("InvestorCommitmentId");
            }
        }

        public int? UniqueCashFlowId
        {
            get
            {
                return this.uniqueCashFlowId;
            }
            set
            {
                this.uniqueCashFlowId = value;
                OnPropertyChanged("UniqueCashFlowId");
            }
        }

        public DateTime EffectiveDate
        {
            get
            {
                return this.effectiveDate;
            }
            set
            {
                this.effectiveDate = value;
                OnPropertyChanged("EffectiveDate");
            }
        }

        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
                OnPropertyChanged("Id");
            }
        }

   
    
        public string CashFlowType
        {
            get
            {
                return this.cashFlowType;
            }
            set
            {
                this.cashFlowType = value;
                OnPropertyChanged("CashFlowType");
            }
        }

        public int CashFlowNumber
        {
            get
            {
                return this.cashFlowNumber;
            }
            set
            {
                this.cashFlowNumber = value;
                OnPropertyChanged("CashFlowNumber");
            }
        }

        public string CashFlowDescription
        {
            get
            {
                return this.cashFlowDescription;
            }
            set
            {
                this.cashFlowDescription = value;
                OnPropertyChanged("CashFlowDescription");
            }
        }

        public double CommitmentAmount
        {
            get
            {
                return this.commitmentAmount;
            }
            set
            {
                this.commitmentAmount = value;
                OnPropertyChanged("CommitmentAmount");
            }
        }

        public double CashFlowAmount
        {
            get
            {
                return this.cashFlowAmount;
            }
            set
            {
                this.cashFlowAmount = value;
                OnPropertyChanged("CashFlowAmount");
            }
        }

        public double ReturnOfCapital
        {
            get
            {
                return this.returnOfCapital;
            }
            set
            {
                this.returnOfCapital = value;
                OnPropertyChanged("ReturnOfCapital");
            }
        }

        public double CapitalGain
        {
            get
            {
                return this.capitalGain;
            }
            set
            {
                this.capitalGain = value;
                OnPropertyChanged("CapitalGain");
            }
        }

        public double OpenPayments
        {
            get
            {
                return this.openPayments;
            }
            set
            {
                this.openPayments = value;
                OnPropertyChanged("Interests");
            }
        }

        public double Dividends
        {
            get
            {
                return this.dividends;
            }
            set
            {
                this.dividends = value;
                OnPropertyChanged("Dividends");
            }
        }

        public double OtherIncome
        {
            get
            {
                return this.otherIncome;
            }
            set
            {
                this.otherIncome = value;
                OnPropertyChanged("OtherIncome");
            }
        }

        public double RecallableAmount
        {
            get
            {
                return this.recallableAmount;
            }
            set
            {
                this.recallableAmount = value;
                OnPropertyChanged("RecallableAmount");
            }
        }

        public double PartnershipExpenses
        {
            get
            {
                return this.partnershipExpenses;
            }
            set
            {
                this.partnershipExpenses = value;
                OnPropertyChanged("PartnershipExpenses");
            }
        }

        public double WithholdingTax
        {
            get
            {
                return this.withholdingTax;
            }
            set
            {
                this.withholdingTax = value;
                OnPropertyChanged("WithholdingTax");
            }
        }

        public double InvestorExpenses
        {
            get
            {
                return this.investorExpenses;
            }
            set
            {
                this.investorExpenses = value;
                OnPropertyChanged("InvestorExpenses");
            }
        }

        public double LookbackInterests
        {
            get
            {
                return this.lookbackInterests;
            }
            set
            {
                this.lookbackInterests = value;
                OnPropertyChanged("LookbackInterests");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null) return;
            PropertyChangedEventHandler newHandler = PropertyChanged;
            newHandler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
