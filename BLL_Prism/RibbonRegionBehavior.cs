using BLL_Infrastructure;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace BLL_Prism
{
    public class RibbonRegionBehavior:RegionBehavior
    {
        public const string BehaviorKey = "RibbonRegionBehavior";
        protected override void OnAttach()
        {
            if (Region.Name == RegionNames.TabControlRegion)
                Region.ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;
        }

        private void ActiveViews_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var tabList = new List<RadRibbonTab>();
                foreach (var view in e.NewItems)
                {
                    foreach (var atr in GetCustomAttributes<RibbonTabAttribute>(view.GetType()))
                    {
                        var tab = Activator.CreateInstance(atr.type) as RadRibbonTab;

                        if (tab is ISupportDataContext && view is ISupportDataContext)

                            ((ISupportDataContext)tab).DataContext = ((ISupportDataContext)view).DataContext;

                        tabList.Add(tab);
                    }
                    tabList.ForEach(x => Region.RegionManager.Regions[RegionNames.RibbonTabRegion].Add(x));
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var viewList = Region.RegionManager.Regions[RegionNames.RibbonTabRegion].Views.ToList();              

                // remove all views exept ApplicationRibbonTab
                foreach(RadRibbonTab view in viewList)
                {
                    if (view.Name == "PermanentRibbonTab") continue;
                    Region.RegionManager.Regions[RegionNames.RibbonTabRegion].Remove(view);
                }
            }
        }

        static IEnumerable<T> GetCustomAttributes<T>(Type type)
        {
            return type.GetCustomAttributes(typeof(T), true).OfType<T>();
        }
    }
}
