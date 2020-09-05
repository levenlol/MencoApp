using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using MencoApp.Core;
using System.Windows.Media;

namespace MencoApp.UI
{
    /// <summary>
    /// Interaction logic for SimConnectionControl.xaml
    /// </summary>
    public partial class SimConnectionControl : UserControl
    {
        public SimConnectionControl()
        {
            InitializeComponent();

            App.GetMencoApp().Sim_AirplaneGeoInformation.PropertyChanged += Sim_AirplaneGeoInformation_PropertyChanged;
            ChangeConnectionStatus();
        }

        private void Sim_AirplaneGeoInformation_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // ensure we access UI resources on main thread.
            try
            {
                Dispatcher.Invoke(new Action(() => ChangeConnectionStatus()));
            }
            catch
            {
                Console.Error.WriteLine("ChangeConnectionStatus error. unable to change background connection image's color");
            }
        }

        private void ChangeConnectionStatus()
        {
            SimConnectionStatus status = App.GetMencoApp().Sim_AirplaneGeoInformation.ConnectionStatus;
            switch (status)
            {
                case SimConnectionStatus.Disconnected:
                    ConnectionStatusImage.Background = new SolidColorBrush(Colors.Red);
                    break;
                case SimConnectionStatus.Tentative:
                    ConnectionStatusImage.Background = new SolidColorBrush(Colors.DarkGoldenrod);
                    break;
                case SimConnectionStatus.Connected:
                    ConnectionStatusImage.Background = new SolidColorBrush(Colors.Green);
                    break;
                default:
                    ConnectionStatusImage.Background = new SolidColorBrush(Colors.Gray);
                    break;
            }

            ReconnectButton.Visibility = status == SimConnectionStatus.Disconnected ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ReconnectButton_Click(object sender, RoutedEventArgs e)
        {
            App.GetMencoApp().Sim_AirplaneGeoInformation.Connect();
            
        }
    }
}