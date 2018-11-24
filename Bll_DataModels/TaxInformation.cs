
using Prism.Mvvm;
using System.ComponentModel.DataAnnotations;

namespace BLL_DataModels
{
    public class TaxInformation :BindableBase
    {
        private int id;
        private int? investorId;
        private int? countryId;
        private string taxStatus;
        private string taxIdentificationNumber;
        private string remarks;

        public virtual Country Country { get; set; }
        public int Id { get => id; set => SetProperty(ref id, value); }
       
        public int?  InvestorId { get => investorId; set => SetProperty(ref investorId, value); }
        public int? CountryId { get => countryId; set=> SetProperty(ref countryId, value); }
        [MaxLength(50)]
        public string  TaxStatus { get => taxStatus; set => SetProperty(ref taxStatus, value); }
        [MaxLength(50)]
        public string TaxIdentificationNumber { get => taxIdentificationNumber; set => SetProperty(ref taxIdentificationNumber, value); }
        [MaxLength(100)]
        public string  Remarks { get => remarks; set=> SetProperty(ref remarks, value); }
    }
}
