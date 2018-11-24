using Prism.Regions;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace BLL_Private_Equity
{
    public class SelectTabAction : TriggerAction<ContentControl>
    {
        protected override void Invoke(object parameter)
        {
            var args = parameter as RoutedEventArgs;
            if (args == null) return;

            var tabItem = FindParent<RadTabItem>(args.OriginalSource as DependencyObject);
            if (tabItem == null) return;

            var tabControl = FindParent<RadTabControl>(tabItem);
            if (tabControl == null) return;

            int position = tabControl.Items.IndexOf(tabItem.Content);
            if (position < 0) return;

            IRegion region = RegionManager.GetObservableRegion(tabControl).Value;            
            if (region == null) return;          

            //Deactivate active Region
            if (region.ActiveViews.Count()==1)
            {
                var activeView = region.ActiveViews.ElementAt(0);
                region.Deactivate(activeView);
            }

            var newView = region.Views.ElementAt(position);
            region.Activate(newView);
        }


        static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
                return null;

            var parent = parentObject as T;
            if (parent != null)
                return parent;

            return FindParent<T>(parentObject);
        }
    }
}
