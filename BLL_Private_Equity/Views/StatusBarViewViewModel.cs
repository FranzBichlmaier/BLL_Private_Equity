using BLL_Private_Equity.Events;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Views
{
    public class StatusBarViewViewModel: BindableBase
    {

        private string statusBarText;
        private readonly IEventAggregator eventAggregator;

        public string StatusBarText
        {
            get { return this.statusBarText; }
            set { SetProperty(ref statusBarText, value); }
        }

        public StatusBarViewViewModel(IEventAggregator eventAggregator)
        {
            StatusBarText = "Bereit";
            this.eventAggregator = eventAggregator;
            eventAggregator.GetEvent<StatusBarEvent>().Subscribe(OnStatusBarEvent);
        }

        private void OnStatusBarEvent(string obj)
        {
            StatusBarText = obj;
        }
    }
}
