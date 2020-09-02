using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MencoApp.Core
{
    public enum UserData
    {
        Dummy,
        Position 
    }

    public static class Constants
    {
        public const int USER_SIMCONNECT = 0x0402;

    }

    public static class Utils
    {
        public static string GetBaseDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string GetResourceDirectory()
        {
            return GetBaseDirectory() + @"Resources\";
        }
    }
}
