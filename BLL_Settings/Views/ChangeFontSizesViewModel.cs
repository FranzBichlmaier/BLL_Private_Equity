using Prism.Regions;
using Telerik.Windows.Controls;

namespace BLL_Settings.Views
{
    public class ChangeFontSizesViewModel: BLL_Prism.HqtBindableBase
    {
        public DelegateCommand FontSizeChangedCommand { get; set; }
        private double smallSize;

        public double SmallSize
        {
            get { return this.smallSize; }
            set { SetProperty(ref smallSize, value); }
        }

        private double normalSize;

        public double NormalSize
        {
            get { return this.normalSize; }
            set { SetProperty(ref normalSize, value); }
        }

        private double largeSize;

        public double LargeSize
        {
            get { return this.largeSize; }
            set { SetProperty(ref largeSize, value); }
        }

        private double newSmall;

        public double NewSmall
        {
            get { return this.newSmall; }
            set { SetProperty(ref newSmall, value); }
        }

        private double newNormal;

        public double NewNormal
        {
            get { return this.newNormal; }
            set { SetProperty(ref newNormal, value); }
        }


        private double newLarge;

        public double NewLarge
        {
            get { return this.newLarge; }
            set { SetProperty(ref newLarge, value); }
        }
        public ChangeFontSizesViewModel()
        {
            TabTitle = "Schriftgröße ändern";

            smallSize = MaterialPalette.Palette.FontSizeS;
            normalSize = MaterialPalette.Palette.FontSize;
            largeSize = MaterialPalette.Palette.FontSizeL;

            NewSmall = smallSize;
            NewNormal = normalSize;
            NewLarge = largeSize;

            FontSizeChangedCommand = new DelegateCommand(OnFontSizeChanged);

        }

        private void OnFontSizeChanged(object obj)
        {
            MaterialPalette.Palette.FontSizeS = NewSmall;
            MaterialPalette.Palette.FontSize = NewNormal;
            MaterialPalette.Palette.FontSizeL = NewLarge;
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }
    }
}
