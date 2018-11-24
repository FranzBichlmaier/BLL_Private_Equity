using System;

namespace BLL_DataModels
{
    public class CommitmentRangeBarChart
    {
        public object DataObject { get; set; }
        public DateTime EventDate { get; set; }
        public string EventCategory { get; set; }
        public double LowValue { get; set; }
        public double HighValue { get; set; }
    }
}
