﻿using MencoApp.Core;
using MencoApp.DB;
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
using System.Windows.Shapes;

namespace MencoApp.UI
{
    /// <summary>
    /// Interaction logic for FlightRouteControl_PH.xaml
    /// </summary>
    public partial class FlightRouteControlPanel : UserControl
    {
        public FlightRouteControlPanel()
        {
            InitializeComponent();

            MencoMap.NewFlightRouteDrawDelegate += OnFlightRouteReady;
            App.GetMencoApp().FlightRouteController.DeleteFlightRouteDelegate += OnFlightRouteDelete;
        }

        private void OnFlightRouteDelete(object sender, FlightRouteEventArgs args)
        {
            foreach(UIElement uiElement in FlightRoutePanelContainer.Children)
            {
                FlightRouteControlTextBlock uiTextBlock = uiElement as FlightRouteControlTextBlock;
                if(uiTextBlock != null && uiTextBlock.FlightRouteName.Text.Equals(args.FlightPlanName))
                {
                    FlightRoutePanelContainer.Children.Remove(uiTextBlock);
                    break;
                }
            }
        }

        // #todo: this should be bound to a UI event. (when the map actually design the route.)
        private void OnFlightRouteReady(object sender, FlightRouteDrawEventArgs args)
        {
            FlightRouteControlTextBlock textBlock = new FlightRouteControlTextBlock();

            textBlock.FlightRouteName.Text = args.FlightRouteData.FlightPlanName;
            textBlock.FlightRouteName.Foreground = new SolidColorBrush(args.RouteColor);
            FlightRoutePanelContainer.Children.Add(textBlock);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AirportData fromAirport, toAirport;
                GetAirportsData(FromAirportTextBox.Text, ToAirportTextBox.Text, out fromAirport, out toAirport);

                App.GetMencoApp().FlightRouteController.RequestFlightRoute(fromAirport.icao, toAirport.icao);
            }
            catch
            {
                MessageBox.Show("Invalid Airports code", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void GetAirportsData(string fromIcao, string toIcao, out AirportData fromAirportData, out AirportData toAirportData)
        {
            //check aircode is icao
            if(fromIcao.Length != 4 || toIcao.Length != 4 || fromIcao == toIcao)
            {
                throw new ArgumentException();
            }

            // try find airport
            AirportsInfoRetriever airports = App.GetMencoApp().IcaoAirports;

            AirportData? fromAirport = airports.GetAirportByIcaoCode(fromIcao);
            AirportData? toAirport = airports.GetAirportByIcaoCode(toIcao);

            if(fromAirport == null || toAirport == null)
            {
                // todo check online if offline check returned false.
                throw new ArgumentException();
            }

            fromAirportData = (AirportData)fromAirport;
            toAirportData = (AirportData)toAirport;
        }
    }
}
