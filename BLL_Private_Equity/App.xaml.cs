
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using BLL_Prism;
using Prism.Modularity;
using BLL_Settings;

namespace BLL_Private_Equity
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
  

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType.FullName;
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var viewModelName = String.Format(CultureInfo.InvariantCulture, $"{ viewName}ViewModel, { viewAssemblyName}");
                return Type.GetType(viewModelName);
            });

            // ensure the formatting of textboxes ... are in German
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");

            FrameworkElement.LanguageProperty.OverrideMetadata(
                              typeof(FrameworkElement),
                              new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            //
            // The following statement defines the location of the viewmodel of a view:
            // it is located in the same namespace as the view using the name: viewname followed by ViewModel
            //

            Bootstrapper bs = new Bootstrapper();
            bs.Run();

        }
    }
}
