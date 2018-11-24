using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BLL_DataModels
{
    public class DistributionView: INotifyPropertyChanged
    {
        private ObservableCollection<InvestorCommitment> investorCommitments;
        private PeFundCashFlow cashFlowInformation;
        private double difference;
        private int distributionNr;
        private int capitalCallNr;
        private double capitalCallAmount;
       
        private double lookBackInterestCall;
        
        

        public DistributionView()
        {
            investorCommitments = new ObservableCollection<InvestorCommitment>();
        }

      

        public double LookBackInterestCall
        {
            get
            {
                return this.lookBackInterestCall;
            }
            set
            {
                this.lookBackInterestCall = value;
                OnPropertyChanged("LookBackInterestCall");
            }
        }



        public ObservableCollection<InvestorCommitment> InvestorCommitments
        {
            get
            {
                return this.investorCommitments;
            }
            set
            {
                this.investorCommitments = value;
                OnPropertyChanged("InvestorCommitments");
            }
        }

        public double CapitalCallAmount
        {
            get
            {
                return this.capitalCallAmount;
            }
            set
            {
                this.capitalCallAmount = value;
                OnPropertyChanged("CapitalCallAmount");
            }
        }

        public int CapitalCallNr
        {
            get
            {
                return this.capitalCallNr;
            }
            set
            {
                this.capitalCallNr = value;
                OnPropertyChanged("CapitalCallNr");
            }
        }

    
        public PeFundCashFlow CashFlowInformation
        {
            get
            {
                return this.cashFlowInformation;
            }
            set
            {
                this.cashFlowInformation = value;
                OnPropertyChanged("CashFlowInformation");
            }
        }

        public double Difference
        {
            get
            {
                return this.difference;
            }
            set
            {
                this.difference = value;
                OnPropertyChanged("Difference");
            }
        }

        public int DistributionNr
        {
            get
            {
                return this.distributionNr;
            }
            set
            {
                this.distributionNr = value;
                OnPropertyChanged("DistributionNr");
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
