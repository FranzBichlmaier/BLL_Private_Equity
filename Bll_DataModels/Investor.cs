using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL_DataModels
{
    public class Investor:INotifyPropertyChanged

    {
        private int id;
       
        private string investorHqTrustAccount=string.Empty;
       
        private string zeichnungsBerechtigung = string.Empty;

        private Name iName = new Name();       
      

        private string investorReference = string.Empty;
        private InvestorGroup group;
 
        private string directoryPath = string.Empty;  

        private DateTime? foundationBirthday = null;
     
        private string finanzamt = string.Empty;
       
        private string steuernummer = string.Empty;
        private string steuerIdentifikation = string.Empty;
        private string sitzDerGesellschaft = string.Empty;
        private string registerGericht = string.Empty;
        private string handelsregisterNummer = string.Empty;

        private bool confidentialLetter=false;
        private bool sendEmail = false;
        private bool sendForm = false;
        private bool isLiqidClient = false;
        private bool isHqtClient = false;
        private bool clientIsOwnAdvisor = false;
        private int? advisorId;
        private int? parentInvestorId;
        private int? currencyId;
        private int? clientAdvisorId;
        private int? taxAdvisorId;
        private int? rechtsNachfolgerVonId;      
        private string companyName = string.Empty;
        private Address privateAddress;
        private Address postalAddress;
        private Mifid statusMifid;
        private Kagb statusKagb;
        private DateTime? fatcaDateAsOff;
        private double wealthAmount;
        private bool passportAvailable = false;
        private bool houseOfCompanyAvailable = false;
        private bool contractAvailable = false;
        private bool signatureListAvailable = false;
        private bool w8BenAvailable = false;
        private bool powerOfAttorneyAvailable = false;
        private bool investorIsFeederStructure = false;
        private bool isFatca = false;
        private bool isCrs = false;
        [JsonIgnore]
        public virtual List<BankAccount> BankAccounts { get; set; }
        [JsonIgnore]
        public virtual List<EMailAccount> EMailAccounts { get; set; }
        [JsonIgnore]
        public virtual Advisor Advisor { get; set; }
        public virtual ClientAdvisor ClientAdvisor { get; set; }
        [ForeignKey("TaxAdvisorId")]
        [JsonIgnore]
        public virtual ClientAdvisor TaxAdvisor { get; set; }
        [JsonIgnore]
        public virtual List<InvestorCommitment> Commitments { get; set; }
        [ForeignKey("ParentInvestorId")]
        [JsonIgnore]
        public virtual List<Investor> RelatedInvestors { get; set; }
        [JsonIgnore]
        public virtual Currency Currency { get; set; }
        [JsonIgnore]
        public virtual List<DocumentAndLetter> DocumentAndLetters { get; set; }
        [JsonIgnore]
        public virtual List<TaxInformation> TaxInformations { get; set; }

        private string displayName;
        [NotMapped]
        public string DisplayName
        {
            get { return this.displayName; }
            set { displayName = value;
                OnPropertyChanged("DisplayName"); }
        }

        public Investor()
        {
            BankAccounts = new List<BankAccount>();
            Commitments = new List<InvestorCommitment>();
            privateAddress = new Address();
            postalAddress = new Address();
            EMailAccounts = new List<EMailAccount>();
            TaxInformations = new List<TaxInformation>();
            RelatedInvestors = new List<Investor>();
            DocumentAndLetters = new List<DocumentAndLetter>();
        }

        public InvestorGroup Group
        {
            get
            {
                return group;
            }
            set
            {
                group = value;
                OnPropertyChanged("Group");
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

        [MaxLength(15)]
        public string InvestorHqTrustAccount
        {
            get
            {
                return this.investorHqTrustAccount;
            }
            set
            {
                this.investorHqTrustAccount = value;
                OnPropertyChanged("InvestorHqTrustAccount");
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

        public string ZeichnungsBerechtigung
        {
            get
            {
                return zeichnungsBerechtigung;
            }
            set
            {
                zeichnungsBerechtigung = value;
                OnPropertyChanged("ZeichnungsBerechtigung");
            }
        }
        public Name IName { get { return iName; } set { iName = value; OnPropertyChanged("IName"); } }

        public string InvestorReference
        {
            get
            {
                return this.investorReference;
            }
            set
            {
                this.investorReference = value;
                OnPropertyChanged("InvestorReference");
            }
        }   
 
        public int? ClientAdvisorId
        {
            get { return clientAdvisorId; }
            set { clientAdvisorId = value; OnPropertyChanged("ClientAdvisorId"); }
        }
        public int? TaxAdvisorId
        {
            get { return taxAdvisorId; }
            set { taxAdvisorId = value; OnPropertyChanged("TaxAdvisorId"); }
        }
        public int? ParentInvestorId
        {
            get
            {
                return this.parentInvestorId;
            }
            set
            {
                this.parentInvestorId = value;
                OnPropertyChanged("InvestorId");
            }
        }
        [MaxLength(15)]
        public string Steuernummer
        {
            get
            {
                return this.steuernummer;
            }
            set
            {
                this.steuernummer = value;
                OnPropertyChanged("Steuernummer");
            }
        }
        [MaxLength(15)]
        public string SteuerIdentifikation
        {
            get
            {
                return this.steuerIdentifikation;
            }
            set
            {
                this.steuerIdentifikation = value;
                OnPropertyChanged("SteuerIdentifikation");
            }
        }
        [MaxLength(100)]
        public string Finanzamt
        {
            get
            {
                return this.finanzamt;
            }
            set
            {
                this.finanzamt = value;
                OnPropertyChanged("Finanzamt");
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

        public DateTime? FoundationBirthday { get { return foundationBirthday; } set { foundationBirthday = value; OnPropertyChanged("FoundationBirthday"); }
        }
  
        [MaxLength(50)]

        public string SitzDerGesellschaft
        {
            get { return sitzDerGesellschaft; }
            set { sitzDerGesellschaft = value; OnPropertyChanged("SitzDerGesellschaft"); }
        }
        [MaxLength(50)]

        public string RegisterGericht
        {
            get { return registerGericht; }
            set { registerGericht = value; OnPropertyChanged("RegisterGericht"); }
        }
        [MaxLength(50)]

        public string HandelsregisterNummer
        {
            get { return handelsregisterNummer; }
            set { handelsregisterNummer = value; OnPropertyChanged("HandelsregisterNummer"); }
        }
        public bool ConfidentialLetter
        {
            get
            {
                return this.confidentialLetter;
            }
            set
            {
                this.confidentialLetter = value;
                OnPropertyChanged("ConfidentialLetter");
            }
        }
        [DefaultValue(false)]
        public bool SendEmail
        {
            get
            {
                return this.sendEmail;
            }
            set
            {
                this.sendEmail = value;
                OnPropertyChanged("SendEmail");
            }
        }

        public bool SendForm
        {
            get
            {
                return this.sendForm;
            }
            set
            {
                this.sendForm = value;
                OnPropertyChanged("SendForm");
            }
        }
        public bool IsHqtClient
        {
            get { return isHqtClient; }
            set { isHqtClient = value; OnPropertyChanged("IsHqtClient"); }
        }

        public bool IsLiqidClient
        {
            get { return isLiqidClient; }
            set { isLiqidClient = value; OnPropertyChanged("IsLiqidClient"); }
        }

        public bool ClientIsOwnAdvisor
        {
            get { return clientIsOwnAdvisor; }
            set { clientIsOwnAdvisor = value;OnPropertyChanged("ClientIsOwnAdvisor"); }
        }

        public int? AdvisorId
        {
            get
            {
                return this.advisorId;
            }
            set
            {
                this.advisorId = value;
                OnPropertyChanged("AdvisorId");
            }
        }
        [MaxLength(100)]
        public string CompanyName
        {
            get
            {
                return this.companyName;
            }
            set
            {
                this.companyName = value;
                OnPropertyChanged("CompanyName");
            }
        }

        public Address PrivateAddress
        {
            get
            {
                return this.privateAddress;
            }
            set
            {
                this.privateAddress = value;
                OnPropertyChanged("PrivateAddress");
            }
        }

        public Address PostalAddress
        {
            get
            {
                return this.postalAddress;
            }
            set
            {
                this.postalAddress = value;
                OnPropertyChanged("PostalAddress");
            }
        }

        public Mifid StatusMifid
        {
            get
            {
                return this.statusMifid;
            }
            set
            {
                this.statusMifid = value;
                OnPropertyChanged("StatusMifid");
            }
        }
        public Kagb StatusKagb
        {
            get
            {
                return this.statusKagb;
            }
            set
            {
                this.statusKagb = value;
                OnPropertyChanged("StatusKagb");
            }
        }
        public DateTime? FatcaDateAsOff { get { return fatcaDateAsOff; } set { fatcaDateAsOff = value; OnPropertyChanged("FatcaDateAsOff"); } }
        public double WealthAmount { get { return wealthAmount; } set { wealthAmount = value; OnPropertyChanged("WealthAmount"); } }
        public bool PassportAvailable { get { return passportAvailable; }  set { passportAvailable = value; OnPropertyChanged("PassportAvailable"); } }
        public bool HouseOfCompanyAvailable { get { return houseOfCompanyAvailable; } set { houseOfCompanyAvailable = value; OnPropertyChanged(" HouseOfCompanyAvailable"); } }
        public bool ContractAvailable { get { return contractAvailable; } set { contractAvailable = value; OnPropertyChanged("ContractAvailable "); } }
        public bool SignatureListAvailable { get { return signatureListAvailable; } set { signatureListAvailable = value; OnPropertyChanged("SignatureListAvailable"); } }
        public bool W8BenAvailable { get { return w8BenAvailable; } set { w8BenAvailable = value; OnPropertyChanged("W8BenAvailable"); } }
        public bool PowerOfAttorneyAvailable { get { return powerOfAttorneyAvailable; } set { powerOfAttorneyAvailable = value; OnPropertyChanged("PowerOfAttorneyAvailable"); } }
        public bool InvestorIsFeederStructur { get { return investorIsFeederStructure; } set { investorIsFeederStructure = value; OnPropertyChanged("InvestorIsFeederStructur"); } }
        public bool IsFatca { get => isFatca; set { isFatca = value; OnPropertyChanged("IsFatca"); } }
        public bool IsCrs { get => isCrs; set { isCrs = value; OnPropertyChanged("IsCrs"); } }
        public int? RechtsNachfolgerVonId { get => rechtsNachfolgerVonId; set { rechtsNachfolgerVonId = value; OnPropertyChanged("RechtsNachFolgerVonId"); } }
        public Investor Copy(Investor source)
        {
            Investor dest = new Investor();
            dest.AdvisorId = source.AdvisorId;
            dest.Advisor = source.Advisor;
            dest.BankAccounts = new List<BankAccount>();
            foreach(BankAccount account in source.BankAccounts)
            {
                dest.BankAccounts.Add(account);
            }
            dest.IName = source.IName;
            dest.ClientIsOwnAdvisor = source.ClientIsOwnAdvisor;
            dest.Commitments = source.Commitments;
            dest.CompanyName = source.CompanyName;
            dest.ConfidentialLetter = source.ConfidentialLetter;
            dest.ContractAvailable = source.ContractAvailable;
            if (source.Currency != null) dest.Currency = source.Currency;
            dest.CurrencyId = source.CurrencyId;  
            dest.DirectoryPath = source.DirectoryPath;
            dest.EMailAccounts = new List<EMailAccount>();
            foreach(EMailAccount email in source.EMailAccounts)
            {
                dest.EMailAccounts.Add(email);
            }
            dest.FatcaDateAsOff = source.FatcaDateAsOff;
            dest.Finanzamt = source.Finanzamt;
            dest.FoundationBirthday = source.FoundationBirthday;
            dest.HouseOfCompanyAvailable = source.HouseOfCompanyAvailable;
            dest.Id = source.Id; 
            dest.InvestorHqTrustAccount = source.InvestorHqTrustAccount;
            dest.ParentInvestorId = source.ParentInvestorId;  
            dest.InvestorReference = source.InvestorReference;  
            dest.IsLiqidClient = source.IsLiqidClient;
            dest.PassportAvailable = source.PassportAvailable;
            dest.PostalAddress = source.PostalAddress;
            dest.PowerOfAttorneyAvailable = source.PowerOfAttorneyAvailable;
            dest.PrivateAddress = source.PrivateAddress;
            dest.SendEmail = source.SendEmail;
            dest.SendForm = source.SendForm;
            dest.SignatureListAvailable = source.SignatureListAvailable;
            dest.StatusKagb = source.StatusKagb;
            dest.StatusMifid = source.StatusMifid;
            dest.SteuerIdentifikation = source.SteuerIdentifikation;
            dest.Steuernummer = source.Steuernummer;  
            dest.TaxInformations = new List<TaxInformation>();
            foreach (TaxInformation tax in source.TaxInformations)
            { dest.TaxInformations.Add(tax); }
            dest.W8BenAvailable = source.W8BenAvailable;
            dest.WealthAmount = source.WealthAmount; 
            dest.InvestorIsFeederStructur = source.InvestorIsFeederStructur;
            dest.ClientAdvisorId = source.ClientAdvisorId;
            dest.TaxAdvisorId = source.TaxAdvisorId;
            dest.IsFatca = source.isFatca;
            dest.IsCrs = source.IsCrs;
            dest.RechtsNachfolgerVonId = source.RechtsNachfolgerVonId;
            return dest;
        }
        [NotMapped]
        public InvestorCollection Owner
        {
            get;
            protected set;
        }
        internal void SetOwner(InvestorCollection collection)
        {
            this.Owner = collection;
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
