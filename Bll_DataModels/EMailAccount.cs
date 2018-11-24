using Prism.Mvvm;
using System.ComponentModel.DataAnnotations;

namespace BLL_DataModels
{
    public class EMailAccount:  BindableBase
    {
        private int id;
        private string emailAddress;
        private string salutation;
        private int? investorId;

        public int Id { get => id; set => SetProperty(ref id, value); }
        [MaxLength (100)]
        public string EmailAddress { get => emailAddress; set => SetProperty(ref emailAddress, value); }
        [MaxLength(100)]
        public string Salutation { get => salutation; set => SetProperty(ref salutation, value); }
        public int? InvestorId { get => investorId; set => SetProperty(ref investorId, value); }

       

    }
}
