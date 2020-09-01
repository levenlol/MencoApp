using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MencoApp.Core
{
    public struct AirplanePosition
    {
        public double altitude;
        public double latitude;
        public double longitude;
    }

    public class AirplaneLocator : AirplaneBaseFunctionality
    {
        public class LocationChangedEventArgs : EventArgs
        {
            public AirplanePosition Position;
        }

        public override string FunctionalityName => "Airplane Locator";

        public static event EventHandler<LocationChangedEventArgs> OnAircraftLocationChanged;

        public AirplaneLocator() : base()
        {
            if(IsConnected)
            {
                simConnection.AddToDataDefinition(UserData.Position, "PLANE ALTITUDE", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnection.AddToDataDefinition(UserData.Position, "PLANE LATITUDE", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnection.AddToDataDefinition(UserData.Position, "PLANE LONGITUDE", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                simConnection.RegisterDataDefineStruct<AirplanePosition>(UserData.Position);

                //simConnection.RequestDataOnSimObjectType(UserData.Position, UserData.Position, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
                simConnection.RequestDataOnSimObject(UserData.Position, UserData.Position, (uint)SIMCONNECT_SIMOBJECT_TYPE.USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 0, 0, 0);
            }
        }

        protected override void SimConnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            base.SimConnect_OnRecvSimobjectDataBytype(sender, data);

            //data.
        }

        protected override void OnRecvObjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            AirplanePosition currentPos = (AirplanePosition) data.dwData[0];
            LocationChangedEventArgs args = new LocationChangedEventArgs { Position = currentPos };

            if (OnAircraftLocationChanged != null)
            {
                OnAircraftLocationChanged(this, args);
            }
        }
    }
}
