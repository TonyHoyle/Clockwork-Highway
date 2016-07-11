using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Diagnostics;

namespace EH.Common
{
    public class EHApi
    {
        private const string ehWeb = "https://www.ecotricity.co.uk/api/ezx/v1/";

        public class EHApiException : Exception
        {
            public EHApiException(string reason) : base(reason)
            {

            }
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

        public class EmailAddress
        {
            public string address { get; set; }
            public string primary { get; set; }
        }

        public class AccountDetails
        {
            public string businessPartnerId { get; set; }
            public string type { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public List<EmailAddress> emailAddresses { get; set; }
            public string street { get; set; }
            public string village { get; set; }
            public string city { get; set; }
            public string postcode { get; set; }
            public object telephoneNumbers { get; set; }
        }

        public class AccountData
        {
            public string id { get; set; }
            public string token { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string verified { get; set; }
            public string businessPartnerId { get; set; }
            public string phone { get; set; }
            public bool electricHighwayAccount { get; set; }
            public AccountDetails accountDetails { get; set; }
            public string googleAPIkey { get; set; }
        }

        private class LoginResult
        {
            public bool result { get; set; }
            public AccountData data { get; set; }
        }

        public class Vehicle
        {
            public string id { get; set; }
            public string registration { get; set; }
            public string specification { get; set; }
            public string model { get; set; }
            public string make { get; set; }
        }

        private class VehicleResult
        {
            public List<Vehicle> result { get; set; }
        }

        public class Connector
        {
            public string compatible { get; set; }
            public string type { get; set; }
            public string status { get; set; }
            public string name { get; set; }
            public string connectorId { get; set; }
            public string sessionDuration { get; set; }
        }

        public class LocationDetails
        {
            public string status { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string name { get; set; }
            public string postcode { get; set; }
            public string location { get; set; }
            public string locationId { get; set; }
            public string pumpId { get; set; }
            public string lastHeartbeat { get; set; }
            public string pumpModel { get; set; }
            public List<Connector> connector { get; set; }
        }

        private class LocationDetailsPump
        {
            public List<LocationDetails> pump { get; set; }
        }

        private class LocationDetailsResult
        {
            public LocationDetailsPump result { get; set; }
        }

        public class Pump
        {
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string name { get; set; }
            public string postcode { get; set; }
            public string location { get; set; }
            public string locationId { get; set; }
            public List<string> pumpId { get; set; }
            public string pumpModel { get; set; }
            public bool available { get; set; }
            public bool swipeOnly { get; set; }
            public double distance { get; set; }
        }

        private class PumpListResult
        {
            public List<Pump> result { get; set; }
        }

        public class ConnectorCost
        {
            public string connectorId { get; set; }
            public string totalCost { get; set; }
            public string baseCost { get; set; }
            public string discountEcoGrp { get; set; }
            public string discountMultiChg { get; set; }
            public string surcharge { get; set; }
            public string freecost { get; set; }
            public string currency { get; set; }
            public string sessionId { get; set; }
            public string sessionDuration { get; set; }
        }

        public class ConnectorDetails
        {
            public string status { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string name { get; set; }
            public string postcode { get; set; }
            public string location { get; set; }
            public string pumpId { get; set; }
            public List<Connector> connector { get; set; }
            public List<ConnectorCost> connectorCost { get; set; }
        }

        private class PumpConnectorsResult
        {
            public ConnectorDetails result { get; set; }
        }
#pragma warning restore 0649

        private async Task<string> ApiCallAsync(string command, Dictionary<string, string> args)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(ehWeb);
            var request = new HttpRequestMessage(HttpMethod.Post, command);

            request.Content = new FormUrlEncodedContent(args);
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new EHApiException("Unable to call "+command+" - "+response.ReasonPhrase);

            string responseString = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(responseString);
            return responseString;
        }

        public async Task<bool> checkDuplicateEmailAsync(string email)
        {
            string apiResult = await ApiCallAsync("checkDuplicateEmail", new Dictionary<string, string>
            {
                { "email", email }
            });
            BoolResult Result = JsonConvert.DeserializeObject<BoolResult>(apiResult);
            return Result.Result;
        }

        public async Task<bool> validateUsernameAsync(string name)
        {
            string apiResult = await ApiCallAsync("validateUsername", new Dictionary<string, string>
            {
                { "name", name }
            });
            BoolResult Result = JsonConvert.DeserializeObject<BoolResult>(apiResult);
            return Result.Result;
        }

        public async Task<List<Address>> getAddressesAsync(string postcode)
        {
            string apiResult = await ApiCallAsync("getAddresses", new Dictionary<string, string>
            {
                { "postcode", postcode }
            });
            AddressResult Result = JsonConvert.DeserializeObject<AddressResult>(apiResult);
            return Result.result;
        }

        public async Task<bool> verifyEmailAsync(string email, string username)
        {
            string apiResult = await ApiCallAsync("verifyEmail", new Dictionary<string, string>
            {
                { "email", email },
                { "username", username }
            });
            BoolResult Result = JsonConvert.DeserializeObject<BoolResult>(apiResult);
            return Result.Result;
        }

        public async Task<AccountData> loginAsync(string username, string password)
        {
            string apiResult = await ApiCallAsync("login", new Dictionary<string, string>
            {
                { "electricHighway", "true" },
                { "identifier", username },
                { "password", password }
            });
            LoginResult Result = JsonConvert.DeserializeObject<LoginResult>(apiResult);
            if (!Result.result)
                return null;
            else
                return Result.data;
        }

        public async Task<List<Vehicle>> getUserVehicleListAsync(string username, string password)
        {
            string apiResult = await ApiCallAsync("getUserVehicleList", new Dictionary<string, string>
            {
                { "identifier", username },
                { "password", password }
            });
            VehicleResult Result = JsonConvert.DeserializeObject<VehicleResult>(apiResult);
            return Result.result;
        }

        public async Task<List<LocationDetails>> getLocationDetailsAsync(int locationId, Vehicle vehicle)
        {
            string apiResult = await ApiCallAsync("getLocationDetails", new Dictionary<string, string>
            {
                { "vehicleSpecification", vehicle.specification },
                { "vehicleModel", vehicle.model },
                { "locationId", locationId.ToString() },
                { "vehicleMake", vehicle.make }
            });
            try
            {
                LocationDetailsResult Result = JsonConvert.DeserializeObject<LocationDetailsResult>(apiResult);
                return Result.result.pump;
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        
        public async Task<List<Pump>> getPumpListAsync(double latitude, double longitude, Vehicle vehicle)
        {
            string apiResult = await ApiCallAsync("getPumpList", new Dictionary<string, string>
            {
                { "vehicleSpec", vehicle.specification },
                { "vehicleMake", vehicle.make },
                { "vehicleModel", vehicle.model },
                { "latitude", latitude.ToString() },
                { "longitude", longitude.ToString() },
            });
            PumpListResult Result = JsonConvert.DeserializeObject<PumpListResult>(apiResult);
            return Result.result;
        }

        public async Task<ConnectorDetails> getPumpConnectorsAsync(string username, string password, int pumpId, string deviceId, Vehicle vehicle)
        {
            string apiResult = await ApiCallAsync("getPumpConnectors", new Dictionary<string, string>
            {
                { "password", password },
                { "deviceId", deviceId },
                { "vehicleId", vehicle.id },
                { "vehicleMake", vehicle.make },
                { "identifier", username },
                { "vehicleModel", vehicle.model },
                { "pumpId", pumpId.ToString() }
            });
            PumpConnectorsResult Result = JsonConvert.DeserializeObject<PumpConnectorsResult>(apiResult);
            return Result.result;
        }
    }
}
