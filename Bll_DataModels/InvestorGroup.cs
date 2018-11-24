using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_DataModels
{
    public enum InvestorGroup
    {
        [Display(Name="Privat-Investor", ShortName = "Privat")]
        HQTrust =0, 
        [Display(Name="Institutioneller Investor", ShortName ="Institutionell")]
        Liqid,
        [Display(Name="PE-Investor", ShortName = "PE-Investor")]
        PeInvestor
    }
}
