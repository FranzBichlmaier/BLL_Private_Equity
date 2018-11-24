using System.ComponentModel;

namespace BLL_DataModels
{
    public class ImportInvestor: INotifyPropertyChanged
    {
        private string investorStatus;
        private int investorId;
        private double commitmentAmount;
        private string comment;
        private Investor investor;

        public ImportInvestor()
        {
            Investor = new Investor();
        }

        public string Comment
        {
            get
            {
                return this.comment;
            }
            set
            {
                this.comment = value;
                OnPropertyChanged("Comment");
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

        public string InvestorStatus
        {
            get
            {
                return this.investorStatus;
            }
            set
            {
                this.investorStatus = value;
                OnPropertyChanged("InvestorStatus");
            }
        }

        public int InvestorId
        {
            get
            {
                return this.investorId;
            }
            set
            {
                this.investorId = value;
                OnPropertyChanged("InvestorId");
            }
        }

        public Investor Investor
        {
            get
            {
                return this.investor;
            }
            set
            {
                this.investor = value;
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
