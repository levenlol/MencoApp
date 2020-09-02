using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MencoApp.Core
{
#if FLIGHT_PLAN_DB
    // see FlightRouteController.cs for more info.
    public struct FlightPlanData
    {
        public long id;
        public string fromICAO;
        public string toICAO;
        public string fromName;
        public string toName;
        public string flightNumber;
        public double distance;
        public double maxAltitude;
        public int waypoints;
        public string notes;
        public DateTime createdAt;
        public DateTime updatedAt;
        public Route route;
    }

    public struct Route // yeah..
    {
        public NavigationNode[] nodes;
    }

    public struct NavigationNode
    {
        public string type;
        public string ident;
        public string name;
        public double lat;
        public double lon;
        public double alt;
    }

    // implementation using https://flightplandatabase.com/
    public sealed class FPDBFlightRouteController : IFlightRouteController
    {
        public event EventHandler<FlightRouteEventArgs> FlightRouteReadyDelegate;

        private HttpClient httpClient;
        public FPDBFlightRouteController()
        {
            // init http client
            httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 0, 10);
        }

        // How it works: Build information and create a route. After the route-request is sent, 
        // we fetch the route with the given id.
        public async void RequestFlightRoute(string fromAirportIcaoCode, string toAirportIcaoCode)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, @"https://api.flightplandatabase.com/auto/generate");

            // Build post Content
            var dict = new Dictionary<string, string>();
            dict.Add("fromICAO", fromAirportIcaoCode);
            dict.Add("toICAO", toAirportIcaoCode);
            dict.Add("fromProc", "Auto");
            dict.Add("toProc", "Auto");
            dict.Add("useNAT", "true");
            dict.Add("useAWYLO", "true");
            dict.Add("usePACOT", "true");
            dict.Add("useAWYHI", "true");

            req.Content = new FormUrlEncodedContent(dict);

            // Set credentials.
            string user = "UXrFMFcg2nTSP2Dc6NDmAcI81UMDX6D9jNV6AebI";
            string pwd = "";

            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(user + ":" + pwd));
            req.Headers.Add("Authorization", "Basic " + encoded);

            // Request
            HttpResponseMessage responseMessage = await httpClient.SendAsync(req);

            if(!responseMessage.IsSuccessStatusCode)
            {
                throw new ArgumentException();
            }

            string response = await responseMessage.Content.ReadAsStringAsync();

            Console.WriteLine("FlightPlan Created");

            // Parse request
            FlightPlanData data = JsonConvert.DeserializeObject<FlightPlanData>(response);

            RequestFlightRoute(data.id);
        }

        // Get route information from id. if valid we fire the delegate. (on main thread.)
        public async void RequestFlightRoute(long id)
        {
            HttpRequestMessage getRequest = new HttpRequestMessage(HttpMethod.Get, $@"https://api.flightplandatabase.com/plan/{id}");

            HttpResponseMessage getResponse = await httpClient.SendAsync(getRequest);
            if (!getResponse.IsSuccessStatusCode)
            {
                throw new ArgumentException();
            }

            string routeMessage = await getResponse.Content.ReadAsStringAsync();

            FlightPlanData data = JsonConvert.DeserializeObject<FlightPlanData>(routeMessage);

            Application.Current?.Dispatcher.Invoke(new Action(() => 
            {
                FlightRouteEventArgs args = new FlightRouteEventArgs(data);
                FlightRouteReadyDelegate?.Invoke(this, args);
            }));
        }

        public async Task<bool> TestConnection()
        {
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
#endif
}
