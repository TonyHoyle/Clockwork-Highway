using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Foundation;
using TonyHoyle.EH;

namespace ClockworkHighway.iOS
{
    public class Shared
    {
 		public static EHApi api { get; set; }
		public static EHApi.Settings settings { get; set; }
		public static HttpClient httpClient { get; set; }
    }
}
