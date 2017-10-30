using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RettSydrome
{
    class Global
    {
        public static int ellipseSize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ellipseSize"]);

        public static string currentSubject;
        public static int currentQuestion;

        public static int current1246;

        public static DateTime expirationDate;
        public static int expirationDays;
        public static bool isIS4Device;

    }
}
