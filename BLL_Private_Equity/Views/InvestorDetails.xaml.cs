using BLL_Prism;
using BLL_Private_Equity.RibbonTabs;
using System.Windows.Controls;

namespace BLL_Private_Equity.Views
{
    /// <summary>
    /// Interaktionslogik für InvestorDetails.xaml
    /// </summary>
    [RibbonTab(typeof(InvestorRibbonTab))]
    public partial class InvestorDetails : UserControl, ICreateRegionManagerScope, ISupportDataContext
    {
        public InvestorDetails()
        {
            InitializeComponent();
        }

        public bool CreateRegionManagerScope => true;

    }
}
