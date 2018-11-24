using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL_DataModels
{
    public class PeFund : INotifyPropertyChanged
    {
        private int id;
        
        private string fundHqTrustNumber;
        
        private string fundName = string.Empty;
        
        private string fundShortName = string.Empty;
        [MaxLength(200)]
        private string fundLegalName = string.Empty;
      
        private string subjectForLetter = string.Empty;
        private string directoryPath = string.Empty;
        [DefaultValue(0)]
        private int? fundManagerId;
        private int? currencyId;
        private int? initiatorId;
        private double fundVolume;
        
        private int vintageYear;
        private bool isFeederFund;
        private bool isExternalAdministrated;
        private bool isHqTrustFund;
        private bool isLiqidFund;
        
        private int? peFundId;
        private int? fundTypeId;
        private int? fundGeographyId;
        private int? fundCompanySizeId;

        public virtual Currency Currency { get; set; }

        [ForeignKey("PeFundId")]
        [JsonIgnore]
        public virtual ObservableCollection<PeFund> FeederFunds { get; set; }
        [JsonIgnore]
        public virtual FundType FundType { get; set; }
        [JsonIgnore]
        public virtual FundCompanySize FundCompanySize { get; set; }
        [JsonIgnore]
        public virtual FundGeography FundGeography { get; set; }

        [ForeignKey("InitiatorId")]
        public virtual Initiator Initiator { get; set; }

        [ForeignKey("FundManagerId")]
        [JsonIgnore]
        public virtual FundManager FundManager { get; set; }

        public virtual ObservableCollection<BankAccount> BankAccounts { get; set; }
        [JsonIgnore]
        public virtual ObservableCollection<PeFundCashFlow> CashFlows { get; set; }
        [JsonIgnore]
        public virtual ObservableCollection<PeFundPcap> Pcaps { get; set; }
        [JsonIgnore]
        public virtual ObservableCollection<DocumentAndLetter> DocumentAndLetters { get; set; }
 
        public PeFund()
        {
            FeederFunds = new ObservableCollection<PeFund>();
            BankAccounts = new ObservableCollection<BankAccount>();
            DocumentAndLetters = new ObservableCollection<DocumentAndLetter>();
            CashFlows = new ObservableCollection<PeFundCashFlow>();
            Pcaps = new ObservableCollection<PeFundPcap>();
        }

        public int? InitiatorId
        {
            get
            {
                return this.initiatorId;
            }
            set
            {
                this.initiatorId = value;
                OnPropertyChanged("InitiatorId");
            }
        }

        [MaxLength(300)]
        public string SubjectForLetter
        {
            get
            {
                return this.subjectForLetter;
            }
            set
            {
                this.subjectForLetter = value;
                OnPropertyChanged("SubjectForLetter");
            }
        }
        [MaxLength(50)]
        public string DirectoryPath
        {
            get
            {
                return this.directoryPath;
            }
            set
            {
                this.directoryPath = value;
                OnPropertyChanged("DirectoryPath");
            }
        }

        [MaxLength(15)]
        public string FundHqTrustNumber
        {
            get
            {
                return this.fundHqTrustNumber;
            }
            set
            {
                this.fundHqTrustNumber = value;
                OnPropertyChanged("FundHqTrustNumber");
            }
        }

        public int VintageYear
        {
            get
            {
                return this.vintageYear;
            }
            set
            {
                this.vintageYear = value;
                OnPropertyChanged("VintageYear");
            }
        }

        public bool IsHqTrustFund
        {
            get
            {
                return this.isHqTrustFund;
            }
            set
            {
                this.isHqTrustFund = value;
                OnPropertyChanged("IsHqTrustFund");
            }
        }
        public bool IsLiqidFund
        {
            get
            {
                return this.isLiqidFund;
            }
            set
            {
                this.isLiqidFund = value;
                OnPropertyChanged("IsLiqidFund");
            }
        }
        public bool IsExternalAdministrated
        {
            get
            {
                return this.isExternalAdministrated;
            }
            set
            {
                this.isExternalAdministrated = value;
                OnPropertyChanged("IsExternalAdministrated");
            }
        }
        public int? FundTypeId
        {
            get
            {
                return this.fundTypeId;
            }
            set
            {
                this.fundTypeId = value;
                OnPropertyChanged("FundTypeId");
            }
        }

        public int? FundGeographyId
        {
            get
            {
                return this.fundGeographyId;
            }
            set
            {
                this.fundGeographyId = value;
                OnPropertyChanged("FundGeographyId");
            }
        }

        public int? FundCompanySizeId
        {
            get
            {
                return this.fundCompanySizeId;
            }
            set
            {
                this.fundCompanySizeId = value;
                OnPropertyChanged("FundCompanySizeId");
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

        [MaxLength(100)]
        public string FundName
        {
            get
            {
                return this.fundName;
            }
            set
            { 
                if (value != this.fundName)
                {
                    ValidationContext validationContext = new ValidationContext(this, null, null);
                    validationContext.MemberName = "FundName";
                    Validator.ValidateProperty(value, validationContext);
                    this.fundName = value;
                    this.OnPropertyChanged("FundName");
                }
            }
        }

        [MaxLength(100)]
        public string FundShortName
        {
            get
            {
                return this.fundShortName;
            }
            set
            {
                this.fundShortName = value;
                OnPropertyChanged("FundShortName");
            }
        }

        [MaxLength(200)]
        public string FundLegalName
        {
            get
            {
                return this.fundLegalName;
            }
            set
            {
                this.fundLegalName = value;
                OnPropertyChanged("FundLegalName");
            }
        }

        public int? FundManagerId
        {
            get
            {
                return this.fundManagerId;
            }
            set
            {
                this.fundManagerId = value;
                OnPropertyChanged("FundManagerId");
            }
        }

        public int? CurrencyId
        {
            get
            {
                return this.currencyId;
            }
            set
            {
                this.currencyId = value;
                OnPropertyChanged("CurrencyId");
            }
        }

        public double FundVolume
        {
            get
            {
                return this.fundVolume;
            }
            set
            {
                this.fundVolume = value;
                OnPropertyChanged("FundVolume");
            }
        }

        public bool IsFeederFund
        {
            get
            {
                return this.isFeederFund;
            }
            set
            {
                this.isFeederFund = value;
                OnPropertyChanged("IsFeederFund");
            }
        }

        public int? PeFundId
        {
            get
            {
                return this.peFundId;
            }
            set
            {
                this.peFundId = value;
                OnPropertyChanged("PeFundId");
            }
        }
        private double positiveIrr;

        public double PositiveIrr
        {
            get { return positiveIrr; }
            set { positiveIrr = value; OnPropertyChanged("PositiveIrr"); }
        }
        private double positiveMM;

        public double PositiveMM
        {
            get { return positiveMM; }
            set { positiveMM = value; OnPropertyChanged("PositiveMM"); }
        }
        private double negativeIrr;

        public double NegativeIrr
        {
            get { return negativeIrr; }
            set { negativeIrr = value; OnPropertyChanged("NegativeIrr"); }
        }
        private double negativeMM;

        public double NegativeMM
        {
            get { return negativeMM; }
            set { negativeMM = value; OnPropertyChanged("NegativeMM"); }
        }

        public PeFund Copy(PeFund source)
        {
            PeFund dest = new PeFund();
            dest.BankAccounts = source.BankAccounts;
            dest.CashFlows = source.CashFlows;
            dest.CurrencyId = source.CurrencyId;
            dest.FundCompanySizeId = source.FundCompanySizeId;
            dest.FundGeographyId = source.FundGeographyId;
            dest.FundHqTrustNumber = source.FundHqTrustNumber;
            dest.FundLegalName = source.FundLegalName;
            dest.FundManagerId = source.FundManagerId;
            dest.FundName = source.FundName;
            dest.FundShortName = source.FundShortName;
            dest.FundTypeId = source.FundTypeId;
            dest.FundVolume = source.fundVolume;
            dest.Id = source.id;
            dest.InitiatorId = source.initiatorId;
            dest.IsExternalAdministrated = source.IsExternalAdministrated;
            dest.IsFeederFund = source.isFeederFund;
            dest.IsHqTrustFund = source.IsHqTrustFund;
            dest.IsLiqidFund = source.IsLiqidFund;
            dest.PeFundId = source.PeFundId;
            dest.SubjectForLetter = source.SubjectForLetter;
            dest.VintageYear = source.VintageYear;
            dest.PositiveIrr = source.PositiveIrr;
            dest.PositiveMM = source.PositiveMM;
            dest.NegativeIrr = source.NegativeIrr;
            dest.NegativeMM = source.NegativeMM;
            dest.DirectoryPath = source.DirectoryPath;

            return dest;

        }
        public override string ToString()
        {
            return FundHqTrustNumber + " " + FundLegalName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;
            PropertyChangedEventHandler newHandler = PropertyChanged;
            newHandler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}