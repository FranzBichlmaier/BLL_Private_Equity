using BLL_Prism;
using BLL_Private_Equity.RibbonTabs;
using System.Windows.Controls;

namespace BLL_Private_Equity.Views
{
    /// <summary>
    /// Interaktionslogik für EditPcaps.xaml
    /// </summary>
    [RibbonTab(typeof(PcapRibbonTab))]
    public partial class EditPcaps : UserControl, ISupportDataContext, ICreateRegionManagerScope
    {
        public EditPcaps()
        {
            InitializeComponent();
        }

        public bool CreateRegionManagerScope => true;
    }
}
