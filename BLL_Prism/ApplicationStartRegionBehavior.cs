using BLL_Infrastructure;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Telerik.Windows.Controls;

namespace BLL_Prism
{
    public class ApplicationStartRegionBehavior : RegionBehavior
    {
        protected override void OnAttach()
        {
            if (Region.Name == RegionNames.InvestorRegion)
                Region.ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;

        }

        private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
        static IEnumerable<T> GetCustomAttributes<T>(Type type)
        {
            return type.GetCustomAttributes(typeof(T), true).OfType<T>();
        }
    }
}
