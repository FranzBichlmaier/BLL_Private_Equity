using Prism.Mvvm;
using Prism.Regions;

namespace BLL_Prism
{
    public class HqtBindableBase : BindableBase, INavigationAware
    {

        private string tabTitle;

        public string TabTitle
        {
            get { return this.tabTitle; }
            set { SetProperty(ref tabTitle, value); }
        }
        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
           
        }
    }
}
