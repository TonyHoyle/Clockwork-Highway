using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace TonyHoyle.EH
{
    public class GoogleApi
    {
        private const string cGeocodeApi = "https://maps.googleapis.com/maps/api/geocode/json?address={0}&region={1}&key={2}";
        private const string cReverseGeocodeApi = "https://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&key={2}";
        private const string cPlaceGeocodeApi = "https://maps.googleapis.com/maps/api/geocode/json?place_id={0}&key={1}";
        private const string cAutocompleteApi = "https://maps.googleapis.com/maps/api/place/autocomplete/json?input={0}&components=country:{1}&key={2}{3}";

        private string _apiKey;
        private HttpClient _httpClient;

#pragma warning disable 0649
        public class AddressComponent
        {
            public string long_name { get; set; }
            public string short_name { get; set; }
            public List<string> types { get; set; }
        }

        public class LatLong
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Bounds
        {
            public LatLong northeast { get; set; }
            public LatLong southwest { get; set; }
        }

        public class Geometry
        {
            public Bounds bounds { get; set; }
            public LatLong location { get; set; }
            public string location_type { get; set; }
            public Bounds viewport { get; set; }
        }

        public class Address
        {
            public List<AddressComponent> address_components { get; set; }
            public string formatted_address { get; set; }
            public Geometry geometry { get; set; }
            public string place_id { get; set; }
            public List<string> types { get; set; }
        }

        private class LookupAddressResult
        {
            public List<Address> results { get; set; }
            public string status { get; set; }
        }

        public class MatchedSubstring
        {
            public int length { get; set; }
            public int offset { get; set; }
        }

        public class Term
        {
            public int offset { get; set; }
            public string value { get; set; }
        }

        public class Prediction
        {
            public string description { get; set; }
            public string id { get; set; }
            public List<MatchedSubstring> matched_substrings { get; set; }
            public string place_id { get; set; }
            public string reference { get; set; }
            public List<Term> terms { get; set; }
            public List<string> types { get; set; }
        }

        public class AutocompleteResult
        {
            public List<Prediction> predictions { get; set; }
            public string status { get; set; }
        }
#pragma warning restore 0649

        public class GoogleApiException : Exception
        {
            public GoogleApiException(string reason) : base(reason)
            {

            }
        }

        public GoogleApi(HttpClient client, string ApiKey)
        {
            _apiKey = ApiKey;
            _httpClient = client;
        }

        private async Task<string> ApiCallAsync(string url, params object[] args)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, String.Format(url, args));

            // Under jelly bean SendAsync can throw a NetworkOnMainThreadException, so
            // we have to do wrap it in a task 
            var response = await Task.Run(() => _httpClient.SendAsync(request));

            if (!response.IsSuccessStatusCode)
                throw new GoogleApiException("Unable to call google API - " + response.ReasonPhrase);

            string responseString = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(responseString);
            return responseString;
        }

        public async Task<List<Address>> lookupAddressAsync(string address, string region = "uk")
        {
            string apiResult = await ApiCallAsync(cGeocodeApi, address.Replace(' ','+'), region, _apiKey);
            LookupAddressResult Result = JsonConvert.DeserializeObject<LookupAddressResult>(apiResult);
            if (Result.status != "OK")
                throw new GoogleApiException("Geocode failed - "+Result.status);
            return Result.results;
        }

        public async Task<List<Prediction>> autocompleteAsync(string address, string region = "uk", LatLong location = null)
        {
            string extra = "";
            if(location != null)
                extra = "&location=" + location.lat.ToString() + "," + location.lng.ToString()+"&radius=5000";
            string apiResult = await ApiCallAsync(cAutocompleteApi, address.Replace(' ','+'), region, _apiKey, extra);
            AutocompleteResult Result = JsonConvert.DeserializeObject<AutocompleteResult>(apiResult);
            if (Result.predictions == null)
                throw new GoogleApiException("Autocomplete failed - " + Result.status);
            return Result.predictions;
        }

        public async Task<List<Address>> lookupLocationAsync(double latitude, double longitude)
        {
            string apiResult = await ApiCallAsync(cReverseGeocodeApi, latitude, longitude, _apiKey);
            LookupAddressResult Result = JsonConvert.DeserializeObject<LookupAddressResult>(apiResult);
            if (Result.status != "OK")
                throw new GoogleApiException("LookupLocation failed - " + Result.status);
            return Result.results;
        }

        public async Task<List<Address>> lookupPlaceIdAsync(string placeId)
        {
            string apiResult = await ApiCallAsync(cPlaceGeocodeApi, placeId, _apiKey);
            LookupAddressResult Result = JsonConvert.DeserializeObject<LookupAddressResult>(apiResult);
            if (Result.status != "OK")
                throw new GoogleApiException("LookupPlaceId failed - " + Result.status);
            return Result.results;
        }
    }
}
