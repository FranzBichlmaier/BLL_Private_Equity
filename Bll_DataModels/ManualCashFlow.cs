using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_DataModels
{
    public class ManualCashFlow: INotifyPropertyChanged
    {
        private DateTime cashFlowDate;
        private string cashFlowType;
        private double cashFlowFundCurrency;
        private double cashFlowInvestorCurrency;
        private bool existingRecord;
        private int investorCommitmentId;        
        private int cashFlowNumber;        
        private string cashFlowDescription;
        private double commitmentAmount;        
        private double returnOfCapital;
        private double capitalGain;
        private double interests;
        private double dividends;
        private double otherIncome;
        private double recallableAmount;
        private double partnershipExpenses;
        private double withholdingTax;
        private double investorExpenses;
        private double lookbackInterests;
        private int id;


        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
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

        public double Interests
        {
            get
            {
                return this.interests;
            }
            set
            {
                this.interests = value;
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

        public bool ExistingRecord
        {
            get
            {
                return this.existingRecord;
            }
            set
            {
                this.existingRecord = value;
                OnPropertyChanged("ExistingRecord");
            }
        }

        public DateTime CashFlowDate
        {
            get
            {
                return this.cashFlowDate;
            }
            set
            {
                this.cashFlowDate = value;
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
            }
        }

        public double CashFlowFundCurrency
        {
            get
            {
                return this.cashFlowFundCurrency;
            }
            set
            {
                this.cashFlowFundCurrency = value;
            }
        }

        public double CashFlowInvestorCurrency
        {
            get
            {
                return this.cashFlowInvestorCurrency;
            }
            set
            {
                this.cashFlowInvestorCurrency = value;
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
