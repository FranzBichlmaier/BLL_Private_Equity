using BLL_Prism;
using BLL_Infrastructure;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Views
{
    public class ApplicationSettingsViewModel: HqtBindableBase
    {
        private readonly IRegionManager regionManager;

       public DelegateCommand<object> SetValuesCommand { get; set; }

        public ApplicationSettingsViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            SetValuesCommand = new DelegateCommand<object>(OnSetValues);
        }

        private void OnSetValues(object obj)
        {
            string parameter = (string)obj;
            switch (parameter)
            {
                case "FontSizes":
                    {
                        regionManager.RequestNavigate(RegionNames.TabControlRegion, ViewNames.ChangeFontSizes);
                        break;
                    }
            }
        }
        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
    }
}
