using BLL_Prism;
using BLL_Private_Equity.RibbonTabs;
using System.Windows.Controls;

namespace BLL_Private_Equity.Views
{

    [RibbonTab(typeof(InvestorCommitmentRibbonTab))]
    /// <summary>
    /// Interaktionslogik für ShowInvestorCommitments.xaml
    /// </summary>
    public partial class ShowInvestorCommitments : UserControl, ISupportDataContext, ICreateRegionManagerScope
    {
        public ShowInvestorCommitments()
        {
            InitializeComponent();
        }

        public bool CreateRegionManagerScope => true;
    }
}
