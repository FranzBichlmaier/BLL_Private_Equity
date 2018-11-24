using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Telerik.Windows.Controls;


namespace BLL_Private_Equity.TelerikHelper
{
    class DataFormTextBoxMultilineField : DataFormDataField
    {
         
        protected override DependencyProperty GetControlBindingProperty()
        {
            return TextBox.TextProperty;
        }
        protected override Control GetControl()
        {
            DependencyProperty dependencyProperty = this.GetControlBindingProperty();
            TextBox textBox = new TextBox();
            if (this.DataMemberBinding != null)
            {
                var binding = this.DataMemberBinding;
                textBox.SetBinding(dependencyProperty, binding);
            }
            textBox.SetBinding(TextBox.IsEnabledProperty, new Binding("IsReadOnly")
            { Source = this, Converter = new InvertedBooleanConverter() });
            textBox.AcceptsReturn = true;
            textBox.TextWrapping = TextWrapping.Wrap;
            return textBox;
        }
    }
}
