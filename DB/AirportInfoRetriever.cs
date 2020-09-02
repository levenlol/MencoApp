using MencoApp.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MencoApp.DB
{
    public struct AirportData
    {
        public string name;
        public string city;
        public string state;
        public string iata;
        public string icao;
        public double latitude;
        public double longitude;
        public char continentID;
        public string refCity;
    }

    public class AirportsInfoRetriever
    {
        List<AirportData> airportsData = new List<AirportData>();
        internal static readonly char[] separator = { '\t' };

        public int AirportsNum => airportsData.Count;

        public void ImportAirportData(string airportFileName)
        {
            string path = Utils.GetResourceDirectory() + airportFileName;
            string[] lines = File.ReadAllLines(path);

            foreach (string s in lines)
            {
                string[] current = s.Split(separator);
                AirportData data;
                data.name = current[0];
                data.city = current[1];
                data.state = current[2];
                data.iata = current[3];
                data.icao = current[4];
                data.latitude = double.Parse(current[5]);
                data.longitude = double.Parse(current[6]);
                data.continentID = current[7][0];
                data.refCity = current[8];

                airportsData.Add(data);
            }
        }

        public AirportData? GetAirportByIcaoCode(string icao)
        {
            int index = airportsData.FindIndex((AirportData airport) => airport.icao.Equals(icao, StringComparison.OrdinalIgnoreCase));
            if(index >= 0)
            {
                return airportsData[index];
            }

            return null;
        }
    }
}
