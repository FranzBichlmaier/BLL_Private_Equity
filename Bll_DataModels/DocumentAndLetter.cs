using Prism.Mvvm;
using System;
using System.ComponentModel.DataAnnotations;

namespace BLL_DataModels
{
    public class DocumentAndLetter: BindableBase

    {
        private int id;
        private DateTime documentDate;
        private DocType documentType;
        private string documentDescription =string.Empty;
        private string documentFileName=string.Empty;
        private int? investorId;
        private int? peFundId;

        public DocType DocumentType { get => documentType; set => SetProperty(ref documentType, value); }

        public int Id { get => id; set => SetProperty(ref id, value); }
        public int? InvestorId { get => investorId; set => SetProperty(ref investorId, value); }
        public int? PeFundId { get => peFundId; set => SetProperty(ref peFundId, value); }

        public DateTime DocumentDate { get => documentDate; set => SetProperty(ref documentDate, value); }

        [MaxLength(100)]
        public string DocumentDescription { get => documentDescription; set => SetProperty(ref documentDescription, value); }
        [MaxLength(100)]
        public string DocumentFileName { get => documentFileName; set => SetProperty(ref documentFileName, value); }
 
    }
}
