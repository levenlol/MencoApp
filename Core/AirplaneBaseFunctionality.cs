using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Timers;
using System.Windows;

namespace MencoApp.Core
{
    public abstract class AirplaneBaseFunctionality
    {
        protected  SimConnect simConnection;
        protected bool PendingActivation = false;

        private Timer timer = new Timer();
        private IntPtr Handle; // not sure for what is used for.

        bool Connected = false;

        protected abstract double PollingInterval { get; }

        public bool IsConnected => Connected && simConnection != null;

        protected AirplaneBaseFunctionality()
        {
            Connect();
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
            if(PendingActivation || IsConnected)
            {
                simConnection.ReceiveMessage();
            }
        }

        ~AirplaneBaseFunctionality()
        {
            Clear();

        }

        private void Connect()
        {
            try
            {
                Handle = new IntPtr();
                simConnection = new SimConnect(FunctionalityName, Handle, Constants.USER_SIMCONNECT, null, 0);

                simConnection.OnRecvOpen += new SimConnect.RecvOpenEventHandler(SimConnect_OnRecvOpen);
                simConnection.OnRecvQuit += new SimConnect.RecvQuitEventHandler(SimConnect_OnRecvQuit);

                // Listen to exceptions
                simConnection.OnRecvException += new SimConnect.RecvExceptionEventHandler(SimConnect_OnRecvException);

                // Catch a simobject data request
                simConnection.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(SimConnect_OnRecvObjectData);
                simConnection.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(SimConnect_OnRecvSimobjectDataBytype);

                PendingActivation = true;

                TryStartTimer();
            }
            catch
            {
                // todo read exception
            }
            
        }

        protected virtual void OnRecvObjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data){}
        private void SimConnect_OnRecvObjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            // invoke on main thread.
            Application.Current?.Dispatcher.Invoke(new Action(() => OnRecvObjectData(sender, data)));
        }

        private void SimConnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            SIMCONNECT_EXCEPTION eException = (SIMCONNECT_EXCEPTION)data.dwException;
            Console.WriteLine("SimConnect_OnRecvException: " + eException.ToString());
        }

        protected virtual void SimConnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            // didn't find any difference between SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE and SIMCONNECT_RECV_EXCEPTION. should be safe to do that.
            // invoke on main thread.
            Application.Current?.Dispatcher.Invoke(new Action(() => OnRecvObjectData(sender, data)));
        }

        private void TryStartTimer()
        {
            if (PollingInterval > 0)
            {
                timer.Interval = PollingInterval;
                timer.Elapsed += Tick;

                timer.Start();
            }
        }

        protected virtual void SimConnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            Console.WriteLine("SimConnect Connected.");

            Connected = true;

            PendingActivation = false;
        }

        protected virtual void SimConnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Console.WriteLine("SimConnect Disconnected.");
            
            timer.Stop();

            Clear();
        }

        private void Clear()
        {
            if (simConnection != null)
            {
                simConnection = null;
            }

            Connected = false;
            PendingActivation = false;
            Handle = IntPtr.Zero;
        }

        public abstract string FunctionalityName { get; }
    }
}
