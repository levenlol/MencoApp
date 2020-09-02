using MencoApp.UI;
using System;
using MencoApp.Core;
using System.Windows;
using System.Threading.Tasks;
using MencoApp.DB;

namespace MencoApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IFlightRouteController FlightRouteController = null;
        public AirportsInfoRetriever IcaoAirports = null;

        public AirplaneLocator Sim_AirplaneLocator = null;

        public static event EventHandler<EventArgs> InitializationCompletedEvent;


        public static App GetMencoApp() { return Current as App; }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var airports = InitAirports();

            Sim_AirplaneLocator = new AirplaneLocator();
            FlightRouteController = new FPDBIFlightRouteController(); // todo. check if we want to use this class or implement another.

            var connection = FlightRouteController.TestConnection();

            await Task.WhenAll(airports, connection);
            IcaoAirports = airports.Result;

            Console.WriteLine("Init Complete");
            InitializationCompletedEvent?.Invoke(this, EventArgs.Empty);
        }

        private async Task<AirportsInfoRetriever> InitAirports()
        {
            AirportsInfoRetriever airports = new AirportsInfoRetriever();
            await Task.Run(() => { airports.ImportAirportData(@"\airports.dat"); } );

            Console.WriteLine($"InitAirports() completed. {airports.AirportsNum} airports imported.");
            return airports;
        }
    }
}
