using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Timers;
using System.Windows;

namespace MencoApp.Core
{
    public abstract class AirplaneBaseFunctionality
    {
        protected  SimConnect simConnection;
        private Timer timer = new Timer();
        private IntPtr Handle; // not sure for what is used for.
        protected abstract double PollingInterval { get; }

        public bool IsConnected =>simConnection != null;

        protected AirplaneBaseFunctionality()
        {
            Connect();

            if(PollingInterval > 0)
            {
                timer.Interval = PollingInterval;
                timer.Elapsed += Tick;

                timer.Start();
            }
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
            if(IsConnected)
            {
                simConnection.ReceiveMessage();
            }
        }

        ~AirplaneBaseFunctionality()
        {
            Clear();

            timer.Stop();
        }

        private void Tick(Object myObj, EventArgs args)
        {
            if(IsConnected)
            {
                simConnection.ReceiveMessage();
            }
        }

        private void Connect()
        {
            try
            {
                Handle = new IntPtr();
                simConnection = new SimConnect(FunctionalityName, Handle, Constants.USER_SIMCONNECT, null, 0);

                simConnection.OnRecvOpen += new SimConnect.RecvOpenEventHandler(SimConnect_OnRecvOpen);
                simConnection.OnRecvQuit += new SimConnect.RecvQuitEventHandler(SimConnect_OnRecvQuit);

                /// Listen to exceptions
                simConnection.OnRecvException += new SimConnect.RecvExceptionEventHandler(SimConnect_OnRecvException);

                simConnection.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(SimConnect_OnRecvObjectData);

                /// Catch a simobject data request
                simConnection.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(SimConnect_OnRecvSimobjectDataBytype);
            }
            catch
            {
                // todo read exception
            }
            
        }

        protected virtual void OnRecvObjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {

        }

        private void SimConnect_OnRecvObjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            Console.WriteLine("Data received, by type");

            // invoke on main thread.
            Application.Current.Dispatcher.Invoke(new Action(() => OnRecvObjectData(sender, data)));
        }

        private void SimConnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            SIMCONNECT_EXCEPTION eException = (SIMCONNECT_EXCEPTION)data.dwException;
            Console.WriteLine("SimConnect_OnRecvException: " + eException.ToString());
        }

        protected virtual void SimConnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            Console.WriteLine("Data received, by type");
        }

        protected virtual void SimConnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            Console.WriteLine("SimConnect Connected.");
        }

        protected virtual void SimConnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Console.WriteLine("SimConnect Disconnected.");

            Clear();
        }

        private void Clear()
        {
            if (simConnection != null)
            {
                simConnection?.Dispose();
                simConnection = null;
            }

            Handle = IntPtr.Zero;
        }

        public abstract string FunctionalityName { get; }
    }
}
