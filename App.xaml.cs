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

        public AirplaneGeoInformationExtrapolator Sim_AirplaneGeoInformation = null;

        public static event EventHandler<EventArgs> InitializationCompletedEvent;

        public static App GetMencoApp() { return Current as App; }

        private void InitFlightRouteController()
        {
#if FLIGHT_PLAN_DB
            FlightRouteController = new FPDBFlightRouteController();
#else
            compilation error.. not yet supported. Please define FLIGHT_PLAN_DB
#endif
        }

        private void InitConnection_Sim()
        {
            Sim_AirplaneGeoInformation = new AirplaneGeoInformationExtrapolator();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            InitFlightRouteController();
            InitConnection_Sim();

            var airports = InitAirports(); // heavy computation
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
