using System.ComponentModel;

namespace BLL_DataModels
{
    public class FundGeography: INotifyPropertyChanged
    {
        
        private int id;
        private string geography;

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

        public string Geography
        {
            get
            {
                return this.geography;
            }
            set
            {
                this.geography = value;
                OnPropertyChanged("Geography");
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
