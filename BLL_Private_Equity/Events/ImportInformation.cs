using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Private_Equity.Events
{
    public class ImportInformation
    {
        public string ImportName { get; set; } = string.Empty;
        public string Information { get; set; } = string.Empty;
        public object Object { get; set; } = null;
    }
}
