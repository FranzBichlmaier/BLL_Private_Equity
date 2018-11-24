using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace BLL_Private_Equity.Converter
{
    public class IbanConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string iban = (string)value;
            return iban;
        }

        /// <summary>
        /// the convertback method is used to format the input. The result is a string of 4-digit bloxks separated by a blank.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string iban = (string)value;
            if (string.IsNullOrEmpty(iban)) return string.Empty;
            iban = iban.Trim();
            iban = iban.Replace(" ", "");

            StringBuilder builder = new StringBuilder();
            int length = 0;
            for (int i = 0; i < iban.Length; i = i + 4)
            {
                if (i + 4 > iban.Length) length = iban.Length - i; else length = 4;
                builder.Append(iban.Substring(i, length) + " ");
            }
            return builder.ToString().Trim();
        }
    }
}
