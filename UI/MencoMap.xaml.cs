using MencoApp.Core;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static MencoApp.Core.AirplaneGeoInformationExtrapolator;

namespace MencoApp.UI
{

    public class FlightRouteDrawEventArgs : EventArgs
    {
        public FlightRouteReadyEventArgs FlightRouteData;
        public Color RouteColor;

        //more info
    }

    public partial class MencoMap : UserControl, INotifyPropertyChanged
    {
        Pushpin airplanePin = null;
        Dictionary<string, List<UIElement>> flightRouteUIElementsMap;
        private Color[] RouteColors = new Color[] { Colors.AliceBlue, Colors.Aqua, Colors.BlueViolet, Colors.DarkOrange, Colors.Magenta, Colors.Yellow, Colors.YellowGreen, Colors.DarkViolet, Colors.LightCoral, Colors.Cyan, Colors.Pink, Colors.Chartreuse, Colors.Chocolate, Colors.PaleGoldenrod };
        private static int ColorIndex = 0;

        private bool gPS;
        public bool GPS 
        {
            get
            {
                return gPS;
            }
            set 
            {
                gPS = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GPS"));
            }
        }

        static public event EventHandler<FlightRouteDrawEventArgs> NewFlightRouteDrawDelegate;
        public event PropertyChangedEventHandler PropertyChanged;

        public MencoMap()
        {
            InitializeComponent();
            DataContext = this;

            InitMap();
            InitPin();

            ShuffleColorArray();

            flightRouteUIElementsMap = new Dictionary<string, List<UIElement>>();

            App mencoApp = App.GetMencoApp();

            mencoApp.Sim_AirplaneGeoInformation.OnAircraftDataChanged += GeoInformationChanged;
            mencoApp.FlightRouteController.FlightRouteReadyDelegate += OnFlightRouteReady;
            mencoApp.FlightRouteController.DeleteFlightRouteDelegate += OnFlightRouteDelete;
        }

        private void ShuffleColorArray()
        {
            Random random = new Random();

            // Knuth shuffle algorithm :: courtesy of Wikipedia :)
            for (int t = 0; t < RouteColors.Length; t++)
            {
                Color tmp = RouteColors[t];
                int r = random.Next(t, RouteColors.Length);
                RouteColors[t] = RouteColors[r];
                RouteColors[r] = tmp;
            }
        }

        private void OnFlightRouteDelete(object sender, FlightRouteEventArgs args)
        {
            if (flightRouteUIElementsMap.ContainsKey(args.FlightPlanName))
            {
                List<UIElement> uiElements = flightRouteUIElementsMap[args.FlightPlanName];
                foreach(UIElement element in uiElements)
                {
                    BingMap.Children.Remove(element);
                }

                flightRouteUIElementsMap.Remove(args.FlightPlanName);
            }
        }

        private void OnFlightRouteReady(object sender, FlightRouteReadyEventArgs args)
        {
            if(flightRouteUIElementsMap.ContainsKey(args.FlightPlanName))
            {
                // we already have a collection describing this particular route.
                // don't need to add a copy.
                return;
            }

            // polyline + markers
            List<UIElement> flightUIElem = new List<UIElement>(args.FlightPlan.waypoints + 1);

            MapPolyline polyline = new MapPolyline();
            Color color = RouteColors[ColorIndex++ % RouteColors.Length];
            polyline.Stroke = new SolidColorBrush(color);
            polyline.StrokeThickness = 5;
            polyline.Opacity = 0.7;

            LocationCollection collection = new LocationCollection();

            for (int i = 0; i < args.FlightPlan.waypoints; i++)
            {
                // fill polyline
                NavigationNode currentNode = args.FlightPlan.route.nodes[i];

                collection.Add(new Location(currentNode.lat, currentNode.lon));

                // add marker
                Pushpin pin = new Pushpin();
                pin.Location = new Location(currentNode.lat, currentNode.lon);
                pin.ToolTip = args.FlightPlan.notes;
                pin.Content = currentNode.type;
                pin.Background = GetColorFromNavigationNodeType(currentNode.type);

                BingMap.Children.Add(pin);
                flightUIElem.Add(pin);
            }

            polyline.Locations = collection;

            BingMap.Children.Add(polyline);
            flightUIElem.Add(polyline);

            flightRouteUIElementsMap.Add(args.FlightPlanName, flightUIElem);

            FlightRouteDrawEventArgs e = new FlightRouteDrawEventArgs();
            e.FlightRouteData = args;
            e.RouteColor = color;

            NewFlightRouteDrawDelegate?.Invoke(this, e);
        }

        private SolidColorBrush GetColorFromNavigationNodeType(string type)
        {
            if (type == "APT")
                return new SolidColorBrush(Colors.OrangeRed);
            else if(type == "FIX")
                return new SolidColorBrush(Colors.BlueViolet);

            return new SolidColorBrush(Colors.DarkOliveGreen);
        }


        private void GeoInformationChanged(object sender, AirplaneGeoInfoChangedEventArgs args)
        {
            double latitude = args.info.latitude;
            double longitude = args.info.longitude;
            double yaw = args.info.yaw;

            MovePin(latitude, longitude, yaw);

            if (GPS)
            {
                BingMap.Center = new Location(latitude, longitude);
            }
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

            GPS = true;
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

        private Location prevCenterLocation;

        private void UserControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            prevCenterLocation = BingMap.Center;
        }

        private void UserControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GPS = prevCenterLocation == BingMap.Center;
        }
    }
}
