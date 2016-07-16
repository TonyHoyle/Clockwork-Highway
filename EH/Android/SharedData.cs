using EH.Common;

namespace EH.Android
{
    static class SharedData
    {
        public static GoogleApi.LatLong lastLocation { get; set; }
        public static EHLogin login { get; set; }
        public static string deviceId;
        public static GoogleApi googleApi { get; set; }
    }
}