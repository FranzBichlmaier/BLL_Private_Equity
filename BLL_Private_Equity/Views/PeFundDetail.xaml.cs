using BLL_Prism;
using BLL_Private_Equity.RibbonTabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BLL_Private_Equity.Views
{
    /// <summary>
    /// Interaktionslogik für PeFundDetail.xaml
    /// </summary>
    [RibbonTab(typeof(PeFundRibbonTab))]
    public partial class PeFundDetail : UserControl, ICreateRegionManagerScope, ISupportDataContext
    {  
        public PeFundDetail()
        {
            InitializeComponent();
        }
        public bool CreateRegionManagerScope => true;
    }
}
