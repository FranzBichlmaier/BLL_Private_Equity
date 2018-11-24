using Prism.Mvvm;

namespace BLL_DataModels
{
    public class InvestorDataErrors:BindableBase
    {
        private Investor investor;
        private string errorMessages;
        private string backGroundColor;
        public Investor Investor { get => investor; set =>SetProperty(ref investor, value); }
        public string ErrorMessages { get => errorMessages; set =>SetProperty(ref errorMessages, value); }

        public string BackGroundColor { get => backGroundColor; set => SetProperty(ref backGroundColor, value); }
       

    }
}
