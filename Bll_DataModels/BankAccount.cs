using Newtonsoft.Json;
using Prism.Mvvm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL_DataModels
{
    public class BankAccount: BindableBase
    {
        private int id;
        
        private string bankName=string.Empty;
        private string bankAddress = string.Empty;
        private string accountNumber = string.Empty;        
        private string bankNumberBlz = string.Empty;       
        private string iban = string.Empty;
        private string bic = string.Empty;
        private string swiftAddress = string.Empty;
        private string signature1 = string.Empty;
        private string signature2 = string.Empty;
        private string accountHolder = string.Empty;
        private int currencyId;
        private string beneficiaryBank = string.Empty;
        private string ffcAccountHolderName = string.Empty;
        private string ffcAccountNumber = string.Empty;
        private string additionalInstructions = string.Empty;
        private int? investorId;
        private int? peFundId;
        private int? bankContactId;
        private string currencyName = string.Empty;
        private string displayName = string.Empty;


        [JsonIgnore]
        public virtual Currency Currency { get; set; }
        [ForeignKey("BankContactId")]
        public virtual ClientAdvisor BankContact { get; set; }

        public int Id { get => id; set => SetProperty(ref id, value ); }

        public int CurrencyId { get => currencyId; set => SetProperty(ref currencyId, value); }

        [MaxLength (75)]
        public string BankName { get => bankName; set => SetProperty(ref bankName,value); }

        [MaxLength(200)]
        public string BankAddress { get => bankAddress; set => SetProperty(ref bankAddress, value); }

        [MaxLength(20)]
        public string AccountNumber { get => accountNumber; set => SetProperty(ref accountNumber, value); }

        [MaxLength(20)]
        public string BankNumberBlz { get => bankNumberBlz; set => SetProperty(ref bankNumberBlz, value); }

        [MaxLength(30)]
        public string Iban { get => iban; set => SetProperty(ref iban, value); }
        [MaxLength(30)]
        public string Bic { get => bic; set => SetProperty(ref bic, value); }
        [MaxLength(15)]
        public string SwiftAddress { get => swiftAddress; set => SetProperty(ref swiftAddress, value); }
        [MaxLength(50)]
        public string BeneficiaryBank { get => beneficiaryBank; set => SetProperty(ref beneficiaryBank, value); }
        [MaxLength(50)]
        public string FfcAccountHolderName { get => ffcAccountHolderName; set => SetProperty(ref ffcAccountHolderName,value); }
        [MaxLength(25)]
        public string  FfcAccountNumber { get => ffcAccountNumber; set => SetProperty(ref ffcAccountNumber,value); }
        public string AdditionalInstructions { get => additionalInstructions; set => SetProperty(ref additionalInstructions, value); }
        [MaxLength(30)]
        public string Signature1 { get => signature1; set => SetProperty(ref signature1,value); }
        [MaxLength(30)]
        public string Signature2 { get => signature2; set=> SetProperty(ref signature2,value); }
        public string AccountHolder { get => accountHolder; set => SetProperty(ref accountHolder, value); }
        public int? InvestorId { get => investorId; set => SetProperty(ref investorId, value); }
        public int? PefundId { get => peFundId; set => SetProperty(ref peFundId, value); }
        public int? BankContactId { get =>bankContactId; set =>SetProperty(ref bankContactId,value); }
        [NotMapped]
        public string CurrencyName { get => currencyName; set => SetProperty(ref currencyName, value); }
        [NotMapped]
        public string DisplayName { get => displayName; set => SetProperty(ref displayName, value); }
    }
}
