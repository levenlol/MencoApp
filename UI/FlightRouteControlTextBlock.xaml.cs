using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MencoApp.UI
{
    /// <summary>
    /// Interaction logic for FlightRouteControlTextBlock.xaml
    /// </summary>
    public partial class FlightRouteControlTextBlock : UserControl
    {
        public FlightRouteControlTextBlock()
        {
            InitializeComponent();
        }

        private void MenuItem_Click_Delete(object sender, RoutedEventArgs e)
        {
            App.GetMencoApp().FlightRouteController.DeleteFlightRoute(FlightRouteName.Text);
        }
    }
}
