using BLL_DataModels;
using BLL_Private_Equity.Berechnungen;
using BLL_Private_Equity.Events;
using BLL_Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;

namespace BLL_Private_Equity.Views
{
    public class PeFundDiagramViewModel: HqtBindableBase
    {

        public ICommand BorderLoadedCommand { get; set; }
        private Border border;
        private SubscriptionToken token;

        PeFund fund = null;     
        RadDiagram diagram;
        SolidColorBrush selectedFundColor = new SolidColorBrush(MaterialPalette.Palette.PrimaryNormalColor);
        SolidColorBrush selectedFundForegroundColor = new SolidColorBrush(MaterialPalette.Palette.PrimaryColor);

        private double width = 200;
        private double height = 100;


        private double startPositionX = 10;               // relativ to top left corner of the diagram
        private double startPositionY = 10;

        private double displacementX = 250;          // position of shapes to the right
        private double displacementY = 200;          // position of shapes to the bottom

        private ObservableCollection<PeFundHirarchy> hirarchies;

        public ObservableCollection<PeFundHirarchy> Hirarchies
        {
            get { return this.hirarchies; }
            set { SetProperty(ref hirarchies, value); }
        }

        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;

        public PeFundDiagramViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            BorderLoadedCommand = new DelegateCommand<object>(OnBorderLoaded);
            token = eventAggregator.GetEvent<ShowPeFundDiagramEvent>().Subscribe(OnShowPeFundDiagram);
        }

        private void OnShowPeFundDiagram(ShowPeFundDiagram obj)
        {
            if (this.fund != null) return;
            if (obj == null) return;
            if (obj.hirarchy == null) return;

            Hirarchies = obj.hirarchy;
            this.fund = obj.fund;
            CreateDiagram();
            if (border != null) border.Child = diagram;
            //eventAggregator.GetEvent<ShowPeFundDiagramEvent>().Unsubscribe(token);
        }

        private void OnBorderLoaded(object obj)
        {
            System.Windows.RoutedEventArgs e = obj as System.Windows.RoutedEventArgs;
            if (e == null) return;
            border = e.Source as Border;
            if (diagram != null)
            {
                border.Child = diagram;
            }
        }

  
        private void CreateDiagram()
        {
            diagram = new RadDiagram();

            // setting top level fund   

            Point nextPoint = new Point()
            {
                X = startPositionX,
                Y = startPositionY
            };

            AddShape(nextPoint, Hirarchies[0], null);

            diagram.Background = new SolidColorBrush(MaterialPalette.Palette.AlternativeColor);

            if (border != null) border.Child = diagram;
        }

        private double AddShape(Point nextPoint, PeFundHirarchy nextLevel, RadDiagramShape sourceShape)
        {
            double newX = 0;
            Point startingPoint = nextPoint;
            RadDiagramShape topLevel = new RadDiagramShape()
            {
                Position = startingPoint,
                Width = width,
                Height = height,
                Content = nextLevel.Fund.FundName
            };
            topLevel.Geometry = ShapeFactory.GetShapeGeometry(CommonShapeType.RectangleShape);
            if (nextLevel.Fund.Id == fund.Id)
            {
                topLevel.Foreground = selectedFundForegroundColor;
                topLevel.Background = selectedFundColor;
            }

            diagram.Items.Add(topLevel);

            if (sourceShape != null)
            {
                // create connection between shape 'Toplevel' and 'SourceShape'

                RadDiagramConnection connection = new RadDiagramConnection();

                connection.Source = sourceShape;
                connection.Target = topLevel;
                connection.ConnectionType = Telerik.Windows.Diagrams.Core.ConnectionType.Polyline;
                connection.SourceCapType = Telerik.Windows.Diagrams.Core.CapType.Arrow1;
                connection.TargetCapType = Telerik.Windows.Diagrams.Core.CapType.None;
                //connection.Content = $"{nextLevel.Fund.FundName} investiert in {sourceShape.Content.ToString()}";

                diagram.AddConnection(connection);
            }

            startingPoint.Y += displacementY;

            foreach (PeFundHirarchy level in nextLevel.Feeder)
            {
                newX = AddShape(startingPoint, level, topLevel);
                startingPoint.X += displacementX;
            }

            return startingPoint.X;
        }

    }
}
