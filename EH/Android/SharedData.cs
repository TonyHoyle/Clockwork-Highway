using EH.Common;

namespace EH.Android
{
    class LatLon
    {
        public double Lat;
        public double Lon;
    };

    static class SharedData
    {
        public static LatLon lastLocation { get; set; }
        public static EHLogin login { get; set; }
    }
}