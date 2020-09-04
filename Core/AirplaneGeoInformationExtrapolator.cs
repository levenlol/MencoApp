using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MencoApp.Core
{
    public struct AirplaneGeoInfo
    {
        // Positions
        public double altitude;
        public double latitude; // deg
        public double longitude; // deg

        // Velocity (m/s)
        public double worldVelocityX;
        public double worldVelocityY;
        public double worldVelocityZ;

        // Rotation DEGREES
        public double yaw;
        public double pitch;
        public double roll;
    }

    public class AirplaneGeoInformationExtrapolator : AirplaneBaseFunctionality
    {
        public class AirplaneGeoInfoChangedEventArgs : EventArgs
        {
            public AirplaneGeoInfo info;
        }

        public override string FunctionalityName => "Airplane Geographic Info Extrapolator";

        protected override double PollingInterval => 0.2;

        public event EventHandler<AirplaneGeoInfoChangedEventArgs> OnAircraftDataChanged;

        public AirplaneGeoInformationExtrapolator() : base()
        {
            if(PendingActivation || IsConnected)
            {
                // Positions
                simConnection.AddToDataDefinition(UserData.Position, "PLANE ALTITUDE", "feet", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnection.AddToDataDefinition(UserData.Position, "PLANE LATITUDE", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnection.AddToDataDefinition(UserData.Position, "PLANE LONGITUDE", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                // Velocity
                simConnection.AddToDataDefinition(UserData.Position, "VELOCITY WORLD X", "Meters per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnection.AddToDataDefinition(UserData.Position, "VELOCITY WORLD Y", "Meters per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnection.AddToDataDefinition(UserData.Position, "VELOCITY WORLD Z", "Meters per second", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                // RotationS
                simConnection.AddToDataDefinition(UserData.Position, "PLANE HEADING DEGREES TRUE", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnection.AddToDataDefinition(UserData.Position, "PLANE PITCH DEGREES", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simConnection.AddToDataDefinition(UserData.Position, "PLANE BANK DEGREES", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                simConnection.RegisterDataDefineStruct<AirplaneGeoInfo>(UserData.Position);

                //simConnection.RequestDataOnSimObjectType(UserData.Position, UserData.Position, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
                simConnection.RequestDataOnSimObject(UserData.Position, UserData.Position, (uint)SIMCONNECT_SIMOBJECT_TYPE.USER, SIMCONNECT_PERIOD.VISUAL_FRAME, SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 0, 0, 0);
            }
        }

        protected override void SimConnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            base.SimConnect_OnRecvSimobjectDataBytype(sender, data);

            //data.
        }

        protected override void OnRecvObjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            AirplaneGeoInfo geoInfo = (AirplaneGeoInfo) data.dwData[0];
            AirplaneGeoInfoChangedEventArgs args = new AirplaneGeoInfoChangedEventArgs { info = geoInfo };

            OnAircraftDataChanged?.Invoke(this, args);
        }
    }
}
