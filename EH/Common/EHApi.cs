using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace TonyHoyle.EH
{
    public class EHApi
    {
        private HttpClient _httpClient;
        private Dictionary<int, ConnectorDetails> _pumpCache = new Dictionary<int, ConnectorDetails>();
        public EHLogin Login { get; set;  }

        private const string ehWeb = "https://www.ecotricity.co.uk/api/ezx/v1/";
        private const string cAppId = "com.ecotricity.electrichighway";

        public class EHApiException : Exception
        {
            public EHApiException(string reason) : base(reason)
            {

            }
        }

        private class SingleOrArrayConverter<T> : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(List<T>));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JToken token = JToken.Load(reader);

                if (token.Type == JTokenType.Null) return null;
                if (token.Type == JTokenType.Array)
                {
                    return token.ToObject<List<T>>();
                }
                return new List<T> { token.ToObject<T>() };
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        public EHApi(HttpClient client)
        {
            _httpClient = client;
            Login = new EHLogin(this);
        }

        public class Terms
        {
            public string title { get; set; }
            public string terms { get; set; }
            public string app_version { get; set; }
        }

#pragma warning disable 0649
        public class BoolResult
        {
            public string message { get; set; }
            public bool result { get; set; }
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
            public List<String> telephoneNumbers { get; set; }
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
			public bool updatedTerms { get; set; }
			public bool marketing { get; set; }
		}

        private class LoginResult
        {
            public string message { get; set; }
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
            public int connectorId { get; set; }
            public string sessionDuration { get; set; }
        }

        public class LocationDetails
        {
            public string status { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public string name { get; set; }
            public string postcode { get; set; }
            public string location { get; set; }
            public int locationId { get; set; }
            public int pumpId { get; set; }
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
            public double latitude { get; set; }
            public double longitude { get; set; }
            public string name { get; set; }
            public string postcode { get; set; }
            public string location { get; set; }
            public int locationId { get; set; }
            public List<int> pumpId { get; set; }
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
            public int connectorId { get; set; }
            public decimal totalCost { get; set; }
            public decimal baseCost { get; set; }
            public decimal discountEcoGrp { get; set; }
            public decimal discountMultiChg { get; set; }
            public decimal surcharge { get; set; }
            public string freecost { get; set; } // boolean?
            public string currency { get; set; }
            public string sessionId { get; set; }
            public int sessionDuration { get; set; }
        }

        public class ConnectorDetails
        {
            public string status { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public string name { get; set; }
            public string postcode { get; set; }
            public string location { get; set; }
            public int pumpId { get; set; }
            [JsonConverter(typeof(SingleOrArrayConverter<Connector>))]
            public List<Connector> connector { get; set; }
            [JsonConverter(typeof(SingleOrArrayConverter<ConnectorCost>))]
            public List<ConnectorCost> connectorCost { get; set; }
        }

        private class PumpConnectorsResult
        {
            public ConnectorDetails result { get; set; }
        }

        public class Card
        {
            public string lastDigits { get; set; }
            public string cardType { get; set; }
            public string cardId { get; set; }
            public string cardIcon { get; set; }
        }

        private class GetCardListResult
        {
            public List<Card> result { get; set; }
        }

        public class ForgottenPasswordHash
        {
            public string hashkey { get; set; }
            public string error { get; set; }
        }

        private class ForgottenPasswordResult
        {
            public ForgottenPasswordHash result { get; set; }
        }

        public class PasswordToken
        {
            public bool success { get; set; }
            public string error { get; set; }
            public string hashkey { get; set; }
        }

        private class GetPasswordTokenResult
        {
            public PasswordToken result { get; set;  }
        }

        public class ChargeStatus
        {
            public string status { get; set; }
            public string message { get; set; }
            public bool completed { get; set; }
            public string cost { get; set; }
            public string sessionId { get; set; }
            public int energyConsumption { get; set; }
            public string started { get; set; }
            public string finished { get; set; }
            public int pumpId { get; set; }
            public int pumpConnector { get; set; }
            public string createdDate { get; set; }
            public string createdTime { get; set; }
        }

        private class GetChargeStatusResult
        {
            public ChargeStatus result { get; set; }
        }

		public class Settings
        {
            public bool result { get; set; }
            public string autocomplete { get; set; }
            public string amplitude { get; set; }
            public string google_maps_key { get; set; }
            public string amplitude_key { get; set; }
            public string defaultChargeCopy { get; set; }
            public string defaultGuestChargeCopy { get; set; }
			public string quoteHeading { get; set; }
			public string quoteCopy { get; set; }
			public string maintenance { get; set; }
			public string maintenanceHeading { get; set; }
			public string maintenanceEnabled { get; set; }
			public string marketingCopy { get; set; }
			public Terms terms { get; set; }
		}

        public class ContractAccount
        {
            public string documentNo { get; set; }
            public string sessionId { get; set; }
            public decimal totalCost { get; set; }
            public string currency { get; set; }
            public string date { get; set; }
            public int pumpId { get; set; }
            public int pumpConnector { get; set; }
            public decimal baseCost { get; set; }
            public decimal discountEcoGrp { get; set; }
            public decimal discountMultiChg { get; set; }
            public decimal surcharge { get; set; }
            public string freeCost { get; set; } // boolean?
        }

        public class ContractTransaction
        {
            public string contractAccountId { get; set; }
            public List<ContractAccount> contractAccount { get; set; }
        }

        // FIXME: Why the extra indirection.. can this be a list?
        private class Transaction
        {
            public ContractTransaction transaction { get; set; }
        }

        private class GetTransactionListResult
        {
            public Transaction result { get; set; }
        }

        public class TokenData
		{
			public string access_token { get; set; }
			public int expires_in { get; set; }
			public string token_type { get; set; }
			public object scope { get; set; }
			public string refresh_token { get; set; }
		}

		public class PricingData
		{
			public string title { get; set; }
			public string value { get; set; }
		}

		public class SessionPricing
		{
			public string title { get; set; }
			public List<PricingData> pricingData { get; set; }
		}

		public class Variable
		{
			public string unitOfMeasurement { get; set; }
			public decimal value { get; set; }
		}

		public class Quote
		{
			public List<SessionPricing> sessionPricing { get; set; }
			public decimal @fixed { get; set; }
			public Variable variable { get; set; }
			public string sessionId { get; set; }
			public string sessionDuration { get; set; }
		}

		public class QuoteResult
		{
			public Quote result { get; set; }
		}

#pragma warning restore 0649

        private async Task<string> ApiCallAsync(string command, Dictionary<string, string> args, bool post = true)
        {
            HttpRequestMessage request;

            request = new HttpRequestMessage(post?HttpMethod.Post:HttpMethod.Get, ehWeb + command);

            if (args != null)
                request.Content = new FormUrlEncodedContent(args);

            // Under jelly bean SendAsync can throw a NetworkOnMainThreadException, so
            // we have to do wrap it in a task 
            var response = await Task.Run(() => _httpClient.SendAsync(request));

            if (!response.IsSuccessStatusCode)
                throw new EHApiException("Unable to call "+command+" - "+response.ReasonPhrase);

            string responseString = await response.Content.ReadAsStringAsync();

            Debug.WriteLine(responseString);
            return responseString;
        }

        public async Task<BoolResult> checkDuplicateEmailAsync(string email)
        {
            string apiResult = await ApiCallAsync("checkDuplicateEmail", new Dictionary<string, string>
            {
                { "email", email }
            });
            BoolResult Result = JsonConvert.DeserializeObject<BoolResult>(apiResult);
            return Result;
        }

        public async Task<BoolResult> validateUsernameAsync(string name)
        {
            string apiResult = await ApiCallAsync("validateUsername", new Dictionary<string, string>
            {
                { "name", name }
            });
            BoolResult Result = JsonConvert.DeserializeObject<BoolResult>(apiResult);
            return Result;
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

        public async Task<BoolResult> verifyEmailAsync(string email, string username)
        {
            string apiResult = await ApiCallAsync("verifyEmail", new Dictionary<string, string>
            {
                { "email", email },
                { "username", username }
            });
            BoolResult Result = JsonConvert.DeserializeObject<BoolResult>(apiResult);
            return Result;
        }

        internal async Task<TokenData> tokenAsync(string username, string password, string deviceId)
        {
			string apiResult = await ApiCallAsync("token", new Dictionary<string, string>
			{
				{ "password", password },
                { "grant_type", "password" },
                { "appId", cAppId },
                { "client_secret", "1363c5f65ec09a2458788e75717a51403b5edd09ae5a40063a53960801305c97"},
                { "deviceId", deviceId },
                { "client_id", "2a66896a0ee4aff229fca61772308e2db24a690316a44bf892d510671ef7f834"},
                { "username", username}
			});
			TokenData Result = JsonConvert.DeserializeObject<TokenData>(apiResult);
            return Result;
		}

		internal async Task<TokenData> tokenAsync(string refresh_token, string deviceId)
		{
			string apiResult = await ApiCallAsync("token", new Dictionary<string, string>
			{
				{ "refresh_token", refresh_token },
				{ "client_secret", "1363c5f65ec09a2458788e75717a51403b5edd09ae5a40063a53960801305c97"},
				{ "grant_type", "refresh_token" },
				{ "deviceId", deviceId },
				{ "client_id", "2a66896a0ee4aff229fca61772308e2db24a690316a44bf892d510671ef7f834"},
				{ "appId", cAppId }
			});
			TokenData Result = JsonConvert.DeserializeObject<TokenData>(apiResult);
			return Result;
		}

        public async Task<AccountData> userAsync()
		{
			string apiResult = await ApiCallAsync("user", new Dictionary<string, string>
					{
                        { "access_token", Login.Token.access_token},
                        { "identifier", Login.Username },
						{ "electricHighway", "true" },
                        { "deviceId", Login.DeviceId },
                        { "appId", cAppId }
					});
			LoginResult Result = JsonConvert.DeserializeObject<LoginResult>(apiResult);
			if (!Result.result)
				return null;
			else
				return Result.data;
		}

		public async Task<List<Vehicle>> getUserVehicleListAsync()
        {
            string apiResult = await ApiCallAsync("getUserVehicleList", new Dictionary<string, string>
            {
                { "identifier", Login.Username },
                { "access_token", Login.Token.access_token }
            });
            VehicleResult Result = JsonConvert.DeserializeObject<VehicleResult>(apiResult);
            return Result.result;
        }

        public async Task<List<LocationDetails>> getLocationDetailsAsync(int locationId)
        {
            string apiResult = await ApiCallAsync("getLocationDetails", new Dictionary<string, string>
            {
                { "vehicleSpecification", Login.Vehicle.specification },
                { "vehicleModel", Login.Vehicle.model },
                { "locationId", locationId.ToString() },
                { "vehicleMake", Login.Vehicle.make }
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
        
        public async Task<List<Pump>> getPumpListAsync(double latitude, double longitude)
        {
            string apiResult = await ApiCallAsync("getPumpList", new Dictionary<string, string>
            {
                { "vehicleSpec", Login.Vehicle.specification },
                { "vehicleMake", Login.Vehicle.make },
                { "vehicleModel", Login.Vehicle.model },
                { "latitude", latitude.ToString() },
                { "longitude", longitude.ToString() },
            });
            PumpListResult Result = JsonConvert.DeserializeObject<PumpListResult>(apiResult);
            return Result.result;
        }

        // Because it's possible to want lots of information about connectors, we cache them to avoid hitting the server constantly
        public bool pumpConnectorsAreCached(int pumpId)
        {
            return _pumpCache.ContainsKey(pumpId);
        }

        public async Task<ConnectorDetails> getPumpConnectorsAsync(int pumpId, bool useCache = true)
        {
            if (useCache && _pumpCache.ContainsKey(pumpId))
                return _pumpCache[pumpId];

            string apiResult = await ApiCallAsync("getPumpConnectors", new Dictionary<string, string>
            {
                { "access_token", Login.Token.access_token },
                { "deviceId", Login.DeviceId },
                { "vehicleId", Login.Vehicle.id },
                { "vehicleMake", Login.Vehicle.make },
                { "identifier", Login.Username },
                { "vehicleModel", Login.Vehicle.model },
                { "pumpId", pumpId.ToString() }
            });
            try
            {
                PumpConnectorsResult Result = JsonConvert.DeserializeObject<PumpConnectorsResult>(apiResult);

                _pumpCache.Remove(pumpId);
                _pumpCache.Add(pumpId, Result.result);
                return Result.result;
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

		public async Task<Quote> quoteAsync(int pumpid, int connectorId)
		{
            string apiResult = await ApiCallAsync("pump/"+pumpid.ToString()+"/connector/"+connectorId.ToString()+"/quote", new Dictionary<string, string>
			{
                { "access_token", Login.Token.access_token },
                { "identifier", Login.Username },
                { "vehicleId", Login.Vehicle.id },
                { "deviceId", Login.DeviceId },
				{ "appId", cAppId }
			});
			try
			{
				QuoteResult Result = JsonConvert.DeserializeObject<QuoteResult>(apiResult);
				return Result.result;
			}
			catch (JsonSerializationException e)
			{
				Debug.WriteLine(e.Message);
				return null;
			}
		}
		
        public async Task<List<Card>> getCardListAsync()
        {
            string apiResult = await ApiCallAsync("getCardList", new Dictionary<string, string>
            {
                { "access_token", Login.Token.access_token },
                { "identifier", Login.Username }
            });
            GetCardListResult Result = JsonConvert.DeserializeObject<GetCardListResult>(apiResult);
            return Result.result;
        }

        public async Task<BoolResult> changePasswordAsync(string oldPassword, string newPassword)
        {
            string apiResult = await ApiCallAsync("changePassword", new Dictionary<string, string>
            {
                { "newPassword", newPassword },
                { "identifier", Login.Username },
                { "password", oldPassword }
            });
            BoolResult Result = JsonConvert.DeserializeObject<BoolResult>(apiResult);
            return Result;
        }

        public async Task<BoolResult> forgottenUsernameAsync(string email)
        {
            string apiResult = await ApiCallAsync("forgottenUsername", new Dictionary<string, string>
            {
                { "email", email }
            });
            BoolResult Result = JsonConvert.DeserializeObject<BoolResult>(apiResult);
            return Result;
        }

        public async Task<ForgottenPasswordHash> forgottenPasswordAsync(string email)
        {
            string apiResult = await ApiCallAsync("forgottenPassword", new Dictionary<string, string>
            {
                { "email", email }
            });
            try
            {
                ForgottenPasswordResult Result = JsonConvert.DeserializeObject<ForgottenPasswordResult>(apiResult);
                return Result.result;
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<PasswordToken> getPasswordTokenAsync(string platform, string hashkey)
        {
            string apiResult = await ApiCallAsync("getPasswordToken", new Dictionary<string, string>
            {
                { "platform", platform },
                { "hashkey", hashkey }
            });
            try
            {
                GetPasswordTokenResult Result = JsonConvert.DeserializeObject<GetPasswordTokenResult>(apiResult);
                return Result.result;
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<BoolResult> usePasswordTokenAsync(string platform, string hashkey1, string hashkey2, string password)
        {
            string apiResult = await ApiCallAsync("usePasswordToken", new Dictionary<string, string>
            {
                { "platform", platform },
                { "hashkey", hashkey1 },
                { "password", password },
                { "forgotpassword_hash_key", hashkey2 },
                { "confirm_password", password }
            });
            try
            {
                BoolResult Result = JsonConvert.DeserializeObject<BoolResult>(apiResult);
                return Result;
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(e.Message);
                return new BoolResult() { result = false, message = e.Message };
            }
        }

        public async Task<BoolResult> changeEmailAsync(string email)
        {
            string apiResult = await ApiCallAsync("changeEmail", new Dictionary<string, string>
            {
                { "newEmail", email },
                { "identifier", Login.Username },
                { "access_token", Login.Token.access_token }
            });
            try
            {
                BoolResult Result = JsonConvert.DeserializeObject<BoolResult>(apiResult);
                return Result;
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(e.Message);
                return new BoolResult() { result = false, message = e.Message };
            }
        }

        public async Task<BoolResult> startChargeSessionAsync(int pumpId, int connectorId, string cvv, string cardId, string sessionId)
        {
            string apiResult = await ApiCallAsync("startChargeSession", new Dictionary<string, string>
            {
                { "access_token", Login.Token.access_token },
                { "deviceId", Login.DeviceId },
                { "identifier", Login.Username },
                { "pumpConnector", connectorId.ToString() },
                { "pumpId", pumpId.ToString() },
                { "cv2", cvv },
                { "cardId", cardId },
                { "sessionId", sessionId }
            });
            try
            {
                BoolResult Result = JsonConvert.DeserializeObject<BoolResult>(apiResult);
                return Result;
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(e.Message);
                return new BoolResult() { result = false, message = e.Message };
            }
        }

        public async Task<BoolResult> stopChargeSessionAsync(int pumpId, int connectorId, string sessionId)
        {
            string apiResult = await ApiCallAsync("stopChargeSession", new Dictionary<string, string>
            {
                { "access_token", Login.Token.access_token },
                { "deviceId", Login.DeviceId },
                { "sessionId", sessionId },
                { "identifier", Login.Username },
                { "pumpConnector", connectorId.ToString() },
                { "pumpId", pumpId.ToString() },
            });
            try
            {
                BoolResult Result = JsonConvert.DeserializeObject<BoolResult>(apiResult);
                return Result;
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(e.Message);
                return new BoolResult() { result = false, message = e.Message };
            }
        }

        public async Task<ChargeStatus> getChargeStatusAsync(int pumpId, int connectorId, string sessionId)
        {
            string apiResult = await ApiCallAsync("getChargeStatus", new Dictionary<string, string>
            {
                { "deviceId", Login.DeviceId },
                { "vehicleMake", Login.Vehicle.make },
                { "sessionId", sessionId },
                { "vehicleModel", Login.Vehicle.model },
                { "pumpConnector", connectorId.ToString() },
                { "pumpId", pumpId.ToString() }
            });
            try
            {
                GetChargeStatusResult Result = JsonConvert.DeserializeObject<GetChargeStatusResult>(apiResult);
                return Result.result;
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(e.Message);
				return null;
            }
        }

        public async Task<ChargeStatus> getChargeStatusAsync()
        {
            string apiResult = await ApiCallAsync("getChargeStatus", new Dictionary<string, string>
            {
                { "identifier", Login.Username },
                { "access_token", Login.Token.access_token },
                { "deviceId", Login.DeviceId }
            });
            try
            {
                GetChargeStatusResult Result = JsonConvert.DeserializeObject<GetChargeStatusResult>(apiResult);
                return Result.result;
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<Settings> getSettingsAsync()
        {
            string apiResult = await ApiCallAsync("getSettings", new Dictionary<string, string>
            {
            });
            try
            {
                Settings Result = JsonConvert.DeserializeObject<Settings>(apiResult);
                return Result;
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<Terms> getTermsAsync()
        {
            string apiResult = await ApiCallAsync("terms?eh=true", null, false);
            try
            {
                Terms Result = JsonConvert.DeserializeObject<Terms>(apiResult);
                return Result;
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<ContractTransaction> getTransactionListAsync()
        {
            string apiResult = await ApiCallAsync("getTransactionList", new Dictionary<string, string>
            {
                { "identifier", Login.Username },
                { "access_token", Login.Token.access_token }
            });
            try
            {
                GetTransactionListResult Result = JsonConvert.DeserializeObject<GetTransactionListResult>(apiResult);
                return Result.result.transaction;
            }
            catch (JsonSerializationException e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

		public async Task<bool> archiveSessionAsync(string sessionId)
		{
			string apiResult = await ApiCallAsync("archiveSession", new Dictionary<string, string>
			{
                { "deviceId", Login.DeviceId },
                { "sessionId", sessionId },
                { "appId", cAppId }
			});
			try
			{
				BoolResult Result = JsonConvert.DeserializeObject<BoolResult>(apiResult);
				return Result.result;
			}
			catch (JsonSerializationException e)
			{
				Debug.WriteLine(e.Message);
                return false;
			}
		}
	}
}
