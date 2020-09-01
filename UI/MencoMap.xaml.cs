﻿using MencoApp.Core;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static MencoApp.Core.AirplaneLocator;

namespace MencoApp.UI
{

    /// <summary>
    /// Interaction logic for MencoMap.xaml
    /// </summary>
    public partial class MencoMap : UserControl
    {
        Pushpin airplanePin = null;

        //Debug
        AirplaneLocator loc = new AirplaneLocator();

        public MencoMap()
        {
            InitializeComponent();

            InitMap();
            InitPin();

            AirplaneLocator.OnAircraftLocationChanged += LocationChange;
        }

        private void LocationChange(object sender, LocationChangedEventArgs args)
        {
            double latitude = args.Position.latitude, longitude = args.Position.longitude;

            MovePin(latitude, longitude);

            //#todo if gps active update map
            BingMap.Center = new Location(latitude, longitude);
        }

        private void MovePin(double latitude, double longitude)
        {
            airplanePin.Location = new Location(latitude, longitude);

            //#todo rotate


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
