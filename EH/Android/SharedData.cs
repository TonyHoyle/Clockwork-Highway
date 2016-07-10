using Android.Gms.Common.Apis;
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
        public static EHApi.AccountData accountData { get; set; }
        public static EHApi.Vehicle vehicle { get; set; }
    }
}