using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using BLL_Private_Equity.Berechnungen;

namespace BLL_Private_Equity.Converter
{
    public class AnalyzingParameterPeriodConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AnalyzingParameterPeriod period= (AnalyzingParameterPeriod)value;
            return Enum.GetName(typeof(AnalyzingParameterPeriod), period);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
