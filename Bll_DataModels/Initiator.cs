using Prism.Mvvm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL_DataModels
{
    public class Initiator: BindableBase
    {
        private int id;
        private int advisorId;
        private Address initiatorAddress;
        private Address headQuarterAddress;
        private string senderInformation;
        private string senderInformationLine2;
        private string initiatorName;
        private string footerLine1;
        private string footerLine2;
        private string footerLine3;

        [ForeignKey("AdvisorId")]
        public virtual Advisor Advisor { get; set; }

        [MaxLength(100)]
        public string InitiatorName { get => initiatorName; set => SetProperty(ref initiatorName, value); }
        public int Id { get => id; set => SetProperty(ref id, value); }

        public int AdvisorId { get => advisorId; set => SetProperty(ref advisorId, value); }

        public Address InitiatorAddress { get => initiatorAddress; set => SetProperty(ref initiatorAddress, value); }
        public Address HeadQuarterAddress { get => headQuarterAddress; set => SetProperty(ref headQuarterAddress, value); }

        [MaxLength(150)]
        public string SenderInformation { get => senderInformation; set => SetProperty(ref senderInformation, value); }
        [MaxLength(150)]
        public string SenderInformationLine2 { get => senderInformationLine2; set => SetProperty(ref senderInformationLine2, value); }
        [MaxLength(200)]
        public string FooterLine1 { get => footerLine1; set => SetProperty(ref footerLine1, value); }
        [MaxLength(200)]
        public string FooterLine2 { get => footerLine2; set => SetProperty(ref footerLine2, value); }
        [MaxLength(200)]
        public string FooterLine3 { get => footerLine3; set => SetProperty(ref footerLine3, value); }

        public Initiator()
        {
            initiatorAddress = new Address();
            headQuarterAddress = new Address();
        }
        /// <summary>
        /// create a copy of an existing initiator
        /// </summary>
        /// <param name="c"></param>
        public Initiator(Initiator c)
        {
            Id = c.Id;
            AdvisorId = c.AdvisorId;
            InitiatorAddress = c.InitiatorAddress;
            HeadQuarterAddress = c.HeadQuarterAddress;
            SenderInformation = c.SenderInformation;
            SenderInformationLine2 = c.SenderInformationLine2;
            InitiatorName = c.InitiatorName;
            FooterLine1 = c.FooterLine1;
            FooterLine2 = c.FooterLine2;
            FooterLine3 = c.FooterLine3;
        }
    }
}
