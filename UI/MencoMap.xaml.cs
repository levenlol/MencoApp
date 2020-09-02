using MencoApp.Core;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static MencoApp.Core.AirplaneGeoInformationExtrapolator;

namespace MencoApp.UI
{

    /// <summary>
    /// Interaction logic for MencoMap.xaml
    /// </summary>
    public partial class MencoMap : UserControl
    {
        Pushpin airplanePin = null;

        public MencoMap()
        {
            InitializeComponent();

            InitMap();
            InitPin();

            App mencoApp = App.GetMencoApp();

            mencoApp.Sim_AirplaneGeoInformation.OnAircraftDataChanged += GeoInformationChanged;
            mencoApp.FlightRouteController.FlightRouteReadyDelegate += OnFlightRouteReady;
        }

        private void OnFlightRouteReady(object sender, FlightRouteEventArgs args)
        {
            MapPolyline polyline = new MapPolyline();
            polyline.Stroke = new SolidColorBrush(Colors.Blue);
            polyline.StrokeThickness = 5;
            polyline.Opacity = 0.7;

            LocationCollection collection = new LocationCollection();

            for (int i = 0; i < args.FlightPlan.waypoints; i++)
            {
                NavigationNode currentNode = args.FlightPlan.route.nodes[i];

                collection.Add(new Location(currentNode.lat, currentNode.lon));

            }

            polyline.Locations = collection;

            BingMap.Children.Add(polyline);
        }


        private void GeoInformationChanged(object sender, AirplaneGeoInfoChangedEventArgs args)
        {
            double latitude = args.info.latitude;
            double longitude = args.info.longitude;
            double yaw = args.info.yaw;

            MovePin(latitude, longitude, yaw);

            //#todo if gps active update map
            BingMap.Center = new Location(latitude, longitude);
        }

        private void MovePin(double latitude, double longitude, double yaw)
        {
            // move
            airplanePin.Location = new Location(latitude, longitude);

            // rotate
            airplanePin.Heading = yaw;
        }

        private void InitMap()
        {
#if DEBUG
            const string ApplicationId = "AowW0iJ0e0NvTAK6zwfWndOq7iRI4pOPq_J0900nALiBApXxD3foFjYhVnFAqsoZ";
#else
	        const string ApplicationId = "qOVqChND2iM7kDwLF8Ef~04jjBqlip6uMVpV-ke9waA~Aknrqgl_hcqEE8DRmjSBmYBeaOlq0sXZBaryxnmTEtcPRVe7HjUojZe72O2bSE-W";
#endif

            ApplicationIdCredentialsProvider credentials = new ApplicationIdCredentialsProvider(ApplicationId);
            BingMap.CredentialsProvider = credentials;
        }

        private void SwitchModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (BingMap.Mode is AerialMode)
            {
                BingMap.Mode = new RoadMode();
                SwitchModeButtonText.Text = "Aerial";
            }
            else if (BingMap.Mode is RoadMode)
            {
                SwitchModeButtonText.Text = "Road";
                BingMap.Mode = new AerialMode(true);
            }
        }

        private void InitPin()
        {
            if(airplanePin == null)
            {
                // add pin
                airplanePin = new Pushpin();
                ControlTemplate template = (ControlTemplate)FindResource("PushpinControlTemplate");

                airplanePin.Location = new Location(45.461488, 9.173296);
                airplanePin.Template = template;

                BingMap.Children.Add(airplanePin);
            }
        }

        private void UserControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // avoid maps going crazy caused by MainWindow calling DragMove()
            e.Handled = true;
        }
    }
}
