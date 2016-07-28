using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace EH.Common
{
    public class Amplitude
    {
        private HttpClient _httpClient;
        private const string cAmplitudeApi = "https://api.amplitude.com/httpapi";
        private string _apiKey;

#pragma warning disable 0649
        public class Event
        {
            public string event_type { get; set; }
            public string device_id { get; set; }
            public string platform { get; set; }
            public string os_version { get; set; }
            public string device_model { get; set; }
            public string device_manufacturer { get; set; }
            public string ip { get; set; }
            public string app_version { get; set; }
            public long time { get; set; }
            public double location_lat { get; set; }
            public double location_lng { get; set; }
            public long session_id { get; set; }
            public string carrier { get; set; }
            public string language { get; set; }
            public string user_id { get; set; }
        }
#pragma warning restore 0649

        public class AmplitudeException : Exception
        {
            public AmplitudeException(string reason) : base(reason)
            {

            }
        }

        public Amplitude(HttpClient client, string ApiKey)
        {
            _apiKey = ApiKey;
            _httpClient = client;
        }

        private async Task<string> ApiCallAsync(Dictionary<string, string> args)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, cAmplitudeApi);

            request.Content = new FormUrlEncodedContent(args);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new AmplitudeException("Unable to call amplitude - " + response.ReasonPhrase);

            string responseString = await response.Content.ReadAsStringAsync();
//            Debug.WriteLine(responseString);
            return responseString;
        }

        public async Task<bool> sendEventAsync(Event ev)
        {
            string apiResult = await ApiCallAsync(new Dictionary<string, string>
            {
                { "event", JsonConvert.SerializeObject(ev) },
                { "api_key", _apiKey }
            });

            return apiResult == "success";
        }
    }
}
