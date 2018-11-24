
using Prism.Mvvm;

namespace BLL_DataModels
{
    public class InvestorToDo: BindableBase
    {
        private int id;
        private int investorId;
        private string todoText;

        public int Id
        {
            get => id; set => SetProperty(ref id, value);           
        }
        public int InvestorId
        {
            get => investorId; set => SetProperty(ref investorId, value);
        }
        public string TodoText
        {
            get => todoText; set => SetProperty(ref todoText, value);
        }
    }
}
