using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MencoApp.Core
{
    public class FlightRouteEventArgs : EventArgs
    {

    }

    public interface IFlightRouteController
    {
        // event fired when a new flight route is ready.
        event EventHandler<FlightRouteEventArgs> FlightRouteReadyDelegate;

        // called when a user prompts a start location and a destination
        void RequestFlightRoute(string fromAirportIcaoCode, string toAirportIcaoCode);

        Task<bool> TestConnection();
    }

    // implementation using https://flightplandatabase.com/
    public sealed class FPDBIFlightRouteController : IFlightRouteController
    {
        public event EventHandler<FlightRouteEventArgs> FlightRouteReadyDelegate;

        public async void RequestFlightRoute(string fromAirportIcaoCode, string toAirportIcaoCode)
        {
            await Task.Delay(1);
            throw new NotImplementedException();
        }

        public async Task<bool> TestConnection()
        {
            // test HTTP:
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 0, 5);

            try
            {
                var response = await httpClient.GetAsync(@"https://api.flightplandatabase.com/");

                Console.WriteLine($"TestConnection() completed with code: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"TestConnection() failed. {e.Message}");
                return false;
            }
        }
    }
}
