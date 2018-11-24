using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace BLL_Private_Equity.Converter
{
    public class DifferenceToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double difference = (double)value;
            SolidColorBrush color = new SolidColorBrush(Telerik.Windows.Controls.MaterialPalette.Palette.MainColor);
            if (difference !=0) color = new SolidColorBrush(Telerik.Windows.Controls.MaterialPalette.Palette.ValidationColor);
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
