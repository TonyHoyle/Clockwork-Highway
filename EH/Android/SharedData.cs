using EH.Common;
using System.Net.Http;

namespace EH.Android
{
    static class SharedData
    {
        public static GoogleApi.LatLong lastLocation { get; set; }
        public static EHLogin login { get; set; }
        public static string deviceId;
        public static GoogleApi googleApi { get; set; }
        public static EHApi.Settings settings { get; set; }
        public static HttpClient httpClient { get; set; }
    }
}