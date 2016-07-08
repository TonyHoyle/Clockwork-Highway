using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

namespace EH.Common
{
    class EHApi
    {
        private const string ehWeb = "https://www.ecotricity.co.uk/api/ezx/v1";

        public class EHApiException : Exception
        {
            private string _reason;

            public EHApiException(String reason) { _reason = reason; }

            public string reason { get; }
        }

#pragma warning disable 0649
        private class BoolResult
        {
            public bool Result { get; set; }
        }

        public class Address
        {
            public string summaryLine { get; set; }
            public string organisation { get; set; }
            public string number { get; set; }
            public string premise { get; set; }
            public string street { get; set; }
            public string posttown { get; set; }
            public string county { get; set; }
            public string postcode { get; set; }
            public string line1 { get; set; }
            public string line2 { get; set; }
            public string line3 { get; set; }
            public string town { get; set; }
        }

        private class AddressResult
        {
            public List<Address> result { get; set; }
        }
#pragma warning restore 0649

        private async Task<string> ApiCallAsync(string command, Dictionary<string, string> args)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(ehWeb);
            var request = new HttpRequestMessage(HttpMethod.Post, "/" + command);

            request.Content = new FormUrlEncodedContent(args);
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new EHApiException("Unable to call "+command+" - "+response.ReasonPhrase);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> checkDuplicateEmailAsync(string email)
        {
            string apiResult = await ApiCallAsync("checkDuplicateEmail", new Dictionary<string, string> { { "email", email } });
            BoolResult Result = JsonConvert.DeserializeObject<BoolResult>(apiResult);
            return Result.Result;
        }

        public async Task<bool> validateUsernameAsync(string name)
        {
            string apiResult = await ApiCallAsync("validateUsername", new Dictionary<string, string> { { "name", name } });
            BoolResult Result = JsonConvert.DeserializeObject<BoolResult>(apiResult);
            return Result.Result;
        }

        public async Task<List<Address>> getAddressesAsync(string postcode)
        {
            string apiResult = await ApiCallAsync("getAddresses", new Dictionary<string, string> { { "postcode", postcode } });
            AddressResult Result = JsonConvert.DeserializeObject<AddressResult>(apiResult);
            return Result.result;
        }

        public async Task<bool> verifyEmailAsync(string email, string username)
        {
            string apiResult = await ApiCallAsync("verifyEmail", new Dictionary<string, string> { { "email", email }, { "username", username } });
            BoolResult Result = JsonConvert.DeserializeObject<BoolResult>(apiResult);
            return Result.Result;
        }
    }
}
