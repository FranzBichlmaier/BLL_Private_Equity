using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BLL_DataModels
{
    public class Address: INotifyPropertyChanged
    {

        private string street;
        private string street2;
        private string zipCode ;        
        private string city ;        
        private string country ; 

        public Address()
        {
            street = string.Empty;
            street2 = string.Empty;
            zipCode = string.Empty;
            city = string.Empty;
            country = string.Empty;
        }
        [MaxLength(100)]
        public string Country
        {
            get
            {
                return this.country;
            }
            set
            {
                this.country = value;
                OnPropertyChanged("Country");
            }
        }
        [MaxLength(100)]
        public string City
        {
            get
            {
                return this.city;
            }
            set
            {
                this.city = value;
                OnPropertyChanged("City");
            }
        }
        [MaxLength(100)]
        public string ZipCode
        {
            get
            {
                return this.zipCode;
            }
            set
            {
                this.zipCode = value;
                OnPropertyChanged("ZipCode");
            }
        }
        [MaxLength(100)]
        public string Street
        {
            get
            {
                return this.street;
            }
            set
            {
                this.street = value;
                OnPropertyChanged("Street");
            }
        }

        public string Street2
        {
            get
            {
                return street2;
            }
            set
            {
                street2 = value;
                OnPropertyChanged("Street2");
            }
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
