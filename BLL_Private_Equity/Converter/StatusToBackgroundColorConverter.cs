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
    /// <summary>
    /// if false return MainBrush
    /// if true return PrimaryNormalBrush
    /// </summary>
    public class StatusToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool status = (bool)value;
            SolidColorBrush color = new SolidColorBrush(Telerik.Windows.Controls.MaterialPalette.Palette.PrimaryNormalColor);
            if (!status) color = new SolidColorBrush(Telerik.Windows.Controls.MaterialPalette.Palette.MainColor);
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
