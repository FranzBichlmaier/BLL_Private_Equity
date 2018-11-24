
using Prism.Mvvm;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL_DataModels
{
    public class Name: BindableBase
    {
        private string title = string.Empty;
       [MaxLength(10)]
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private string firstName = string.Empty;
        [MaxLength(30)]
        public string FirstName
        {
            get { return firstName; }
            set { SetProperty(ref firstName, value); }
        }

        private string lastName = string.Empty;
        [MaxLength(50)]
        public string LastName
        {
            get { return lastName; }
            set { SetProperty(ref lastName, value); }
        }

        private string addressName = string.Empty;
        [MaxLength(50)]
        public string AddressName
        {
            get { return addressName; }
            set { SetProperty(ref addressName, value); }
        }
        private string salutation = string.Empty;
        [MaxLength(70)]
        public string Salutation
        {
            get { return salutation; }
            set { SetProperty(ref salutation, value); }
        }
        
        private string telephoneNumber = string.Empty;
        
        [MaxLength(20)]
        public string TelephoneNumber
        {
            get { return telephoneNumber; }
            set { SetProperty(ref telephoneNumber, value); }
        }
        private string  telephoneNumber2 = string.Empty;

        public string  TelephoneNumber2
        {
            get { return telephoneNumber2 ; }
            set { SetProperty(ref telephoneNumber2, value); }
        }

        private string faxNumber = string.Empty;
       
        [MaxLength(20)]
        public string FaxNumber
        {
            get { return faxNumber; }
            set { SetProperty(ref faxNumber, value); }
        }
        private string emailAddress = string.Empty;
        
        [MaxLength(70)]
        public string EmailAddress
        {
            get { return emailAddress; }
            set { SetProperty(ref emailAddress, value);  }
        }


        private string fullName;
        [NotMapped]

        public string FullName
        {
            get { return this.fullName; }
            set { SetProperty(ref fullName, value); }
        }



    }
}
