using TonyHoyle.EH;
using System.Net.Http;

namespace ClockworkHighway.Android
{
    static class SharedData
    {
		public const string APP = "ClockworkHighway";

		public static GoogleApi.LatLong lastLocation { get; set; }
        public static EHApi api { get; set; }
        public static GoogleApi googleApi { get; set; }
        public static EHApi.Settings settings { get; set; }
        public static HttpClient httpClient { get; set; }
    }
}