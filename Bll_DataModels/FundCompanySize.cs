using System.ComponentModel;

namespace BLL_DataModels
{
    public  class FundCompanySize: INotifyPropertyChanged
    {
        private int id;
        private string companySize;

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

        public string CompanySize
        {
            get
            {
                return this.companySize;
            }
            set
            {
                this.companySize = value;
                OnPropertyChanged("CompanySize");
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
