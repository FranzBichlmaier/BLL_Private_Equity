
using Prism.Mvvm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL_DataModels
{
    public class ClientAdvisor: BindableBase
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }
        private string companyName =string.Empty;
        [MaxLength(50)]
        public string CompanyName
        {
            get { return companyName; }
            set { SetProperty(ref companyName, value); }
        }
        private Name advisorName;

        public Name AdvisorName
        {
            get { return advisorName; }
            set { SetProperty(ref advisorName, value); }
        }
        private Address address;

        public Address Address
        {
            get { return address; }
            set { SetProperty(ref address, value); }
        }

        private string displayName =string.Empty;
        [NotMapped]
        public string DisplayName
        {
            get { return displayName ; }
            set { SetProperty(ref displayName, value); }
        }

        private string department = string.Empty;
        [MaxLength(50)]
        public string Department
        {
            get { return department; }
            set { SetProperty(ref department, value); }
        }
        private string position = string.Empty;
        [MaxLength(30)]
        public string Position
        {
            get { return position; }
            set { SetProperty(ref position, value); }
        }

        private int earlyNotification = 0;
       
        public int EarlyNotification
        {
            get { return earlyNotification; }
            set { SetProperty(ref earlyNotification, value);  }
        }

        private bool isTaxAdvisor;

        public bool IsTaxAdvisor
        {
            get { return this.isTaxAdvisor; }
            set { SetProperty(ref isTaxAdvisor, value); }
        }


        private bool isClient;

        public bool IsClient
        {
            get { return this.isClient; }
            set { SetProperty(ref isClient, value); }
        }


        private bool isAdvisor;

        public bool IsAdvisor
        {
            get { return this.isAdvisor; }
            set { SetProperty(ref isAdvisor, value); }
        }
        public ClientAdvisor()
        {
            address = new Address();
            advisorName = new Name();

        }
        public ClientAdvisor(ClientAdvisor adv)
        {
            Address = new Address();
            AdvisorName = new Name();
            Address.Street = adv.Address.Street;
            Address.City = adv.Address.City;
            Address.Country = adv.Address.Country;
            Address.ZipCode = adv.Address.ZipCode;
            AdvisorName.AddressName = adv.AdvisorName.AddressName;
            AdvisorName.EmailAddress = adv.AdvisorName.EmailAddress;
            AdvisorName.FaxNumber = adv.AdvisorName.FaxNumber;
            AdvisorName.FirstName = adv.AdvisorName.FirstName;
            AdvisorName.LastName = adv.AdvisorName.LastName;
            AdvisorName.Salutation = adv.AdvisorName.Salutation;
            AdvisorName.TelephoneNumber = adv.AdvisorName.TelephoneNumber;
            AdvisorName.TelephoneNumber2 = adv.AdvisorName.TelephoneNumber2;
            AdvisorName.Title = adv.AdvisorName.Title;
            CompanyName = adv.CompanyName;
            Department = adv.Department;
            EarlyNotification = adv.EarlyNotification;
            Id = adv.Id;
            Position = adv.Position;
            IsAdvisor = adv.IsAdvisor;
            IsClient = adv.IsClient;
            IsTaxAdvisor = adv.IsTaxAdvisor;
        }

        public ClientAdvisor CopyRecord(ClientAdvisor adv)
        {
            ClientAdvisor target = new ClientAdvisor();
            target.Address.Street = adv.Address.Street;
            target.Address.City = adv.Address.City;
            target.Address.Country = adv.Address.Country;
            target.Address.ZipCode = adv.Address.ZipCode;
            target.AdvisorName.AddressName = adv.AdvisorName.AddressName;
            target.AdvisorName.EmailAddress = adv.AdvisorName.EmailAddress;
            target.AdvisorName.FaxNumber = adv.AdvisorName.FaxNumber;
            target.AdvisorName.FirstName = adv.AdvisorName.FirstName;
            target.AdvisorName.LastName = adv.AdvisorName.LastName;
            target.AdvisorName.Salutation = adv.AdvisorName.Salutation;
            target.AdvisorName.TelephoneNumber = adv.AdvisorName.TelephoneNumber;
            target.AdvisorName.TelephoneNumber2 = adv.AdvisorName.TelephoneNumber2;
            target.AdvisorName.Title = adv.AdvisorName.Title;            
            target.CompanyName = adv.CompanyName;
            target.Department = adv.Department;
            target.EarlyNotification = adv.EarlyNotification;
            target.Id = adv.Id;
            target.Position = adv.Position;
            target.IsAdvisor = adv.IsAdvisor;
            target.IsClient = adv.IsClient;
            target.IsTaxAdvisor = adv.IsTaxAdvisor;

            return target;
        }
        public override string ToString()
        {
            return this.AdvisorName.Title + " " + this.AdvisorName.FirstName + " " + this.AdvisorName.LastName + " " + this.CompanyName;
        }
    }
}
