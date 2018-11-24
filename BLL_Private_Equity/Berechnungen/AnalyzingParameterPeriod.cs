using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Berechnungen
{
    public enum AnalyzingParameterPeriod
    {
        [Display(Name = "Automatisch", ShortName = "auto")]
        Auto =0,
        [Display(Name = "Quartal", ShortName = "Quartal")]
        Quarter ,
        [Display(Name = "Halbjahr", ShortName = "6 Monate")]
        SixMonth,
        [Display(Name = "Jahr", ShortName = "12 Monate")]
        Year,
        [Display(Name = "2 Jahre", ShortName = "24 Monate")]
        TwoYear,
        [Display(Name = "5 Jahre", ShortName = "60 Monate")]
        FiveYear
    }
}
