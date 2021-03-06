﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MencoApp.Core
{
    // Define data structure that describes the FlightPlan
    // public struct FlightPlanData MUST be defined. (will not compile otherwise)
    // It own usage and implementation should be made specific according to currently used api.
#if !FLIGHT_PLAN_DB
    // empty implementation.
    public struct FlightPlanData
    {
        //#future: offline implementation.
    }
#endif

    public class FlightRouteEventArgs : EventArgs
    {
        public string FlightPlanName;
        public FlightRouteEventArgs(string name)
        {
            FlightPlanName = name;
        }
    }

    public class FlightRouteReadyEventArgs : FlightRouteEventArgs
    {
        public FlightPlanData FlightPlan;

        public FlightRouteReadyEventArgs(string name, FlightPlanData data) : base(name)
        {
            FlightPlan = data;
        }
    }

    public interface IFlightRouteController
    {
        // event fired when a new flight route is ready.
        event EventHandler<FlightRouteReadyEventArgs> FlightRouteReadyDelegate;
        event EventHandler<FlightRouteEventArgs> DeleteFlightRouteDelegate;

        // called when a user prompts a start location and a destination
        void RequestFlightRoute(string fromAirportIcaoCode, string toAirportIcaoCode);
        void RequestFlightRoute(long id);

        void DeleteFlightRoute(string routeName);

        Task<bool> TestConnection();
    }
}
