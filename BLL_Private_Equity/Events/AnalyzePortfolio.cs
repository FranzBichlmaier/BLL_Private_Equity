using BLL_Private_Equity.Berechnungen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Events
{
    public class AnalyzePortfolio
    {
        public List<ExtendedPcap> Pcaps { get; set; }
        public AnalyzingParameter AnalyzingParameter { get; set; }
        public PeFundResults PeFundResults { get; set; }
    }
}
