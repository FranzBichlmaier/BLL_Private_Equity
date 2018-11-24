using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_DataModels
{
    public class ReportNumber
    {
        public int Id { get; set; }
        public DateTime DateReportGenerated { get; set; }
        public string ReportName { get; set; }
    }
}
