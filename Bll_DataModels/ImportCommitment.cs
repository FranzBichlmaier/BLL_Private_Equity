using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL_DataModels
{
    public class ImportCommitment
    {
        public int Id { get; set; }
        [MaxLength(15)]
        public string PeFundNumber { get; set; }
        [MaxLength(5)]
        public string PeFundCurrency { get; set; }
        public int PeFundCurrencyId { get; set; }
        [MaxLength(50)]
        public string PeFundName { get; set; }
        [MaxLength(15)]
        public string InvestorNumber { get; set; }
        [MaxLength(5)]
        public string InvestorCurrency { get; set; }
        public int InvestorCurrencyId { get; set; }
        public DateTime AsOfDate { get; set; }
        public double Commitment { get; set; }
        public int PeFundId { get; set; }
        public int InvestorId { get; set; }
        public int InvestorCommitmentId { get; set; }
        [NotMapped]
        public string ErrorInformation { get; set; } = string.Empty;
        [NotMapped]
        public bool FoundInPsPlus { get; set; } = false;
        [NotMapped]
        public bool CommitmentsAdded { get; set; } = false;
        [NotMapped]
        public double HqpeCommitment { get; set; }
    }
}
