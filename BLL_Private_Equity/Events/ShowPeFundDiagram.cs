using BLL_DataModels;
using BLL_Private_Equity.Berechnungen;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Events
{
    public class ShowPeFundDiagram
    {
        public ObservableCollection<PeFundHirarchy> hirarchy;
        public PeFund fund; 
    }
}
