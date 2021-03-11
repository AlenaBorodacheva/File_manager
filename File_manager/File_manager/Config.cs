using System;
using System.Configuration;

namespace File_manager
{
    class Config
    {
        static string depthStr = ConfigurationManager.AppSettings["depth"]; // глубина вывода каталога
        public static string Depth()
        {
            if (Int32.TryParse(depthStr, out _))
                return depthStr;
            else
                return "2";
        }

        static string countStr = ConfigurationManager.AppSettings["str_count"]; // количество строк на одной странице
        public static string StrCount()
        {
            if (Int32.TryParse(countStr, out _))
                return countStr;
            else
                return "15";
        }

        static string maxHorStr = ConfigurationManager.AppSettings["max_horizontal"];  // максимальный горизонтальный размер окна
        public static string StrMaxHor()
        {
            if (Int32.TryParse(maxHorStr, out _))
                return maxHorStr;
            else
                return "150";
        }
    }
}
