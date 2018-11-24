using System.ComponentModel;

namespace BLL_DataModels
{
    public class FundManager: INotifyPropertyChanged
    {
        public int id;
        public string fundManagerName; 

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

        public string FundManagerName
        {
            get
            {
                return this.fundManagerName;
            }
            set
            {
                this.fundManagerName = value;
                OnPropertyChanged("FundManagerName");
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
