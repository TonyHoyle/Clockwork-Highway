using System.Collections.Generic;
using System.Threading.Tasks;

namespace TonyHoyle.EH
{
    public class EHLogin
    {
        public bool IsLoggedIn { get; private set; }
        public List<EHApi.Vehicle> Vehicles { get; private set; }
        public List<EHApi.Card> Cards { get; private set; }
        public string Username { get; private set; }
        public EHApi.TokenData Token { get; private set; }
        public EHApi.AccountData Account { get; private set; }
        public string DeviceId { get; private set; }
        public int DefaultVehicleIndex { get; set; }
        public int DefaultCardIndex { get; set; }
        private EHApi _api;

        public EHApi.Vehicle Vehicle {
            get
            {
                if (DefaultVehicleIndex >= 0 && DefaultVehicleIndex < Vehicles.Count)
                    return Vehicles[DefaultVehicleIndex];
                else if (Vehicles.Count > 0)
                    return Vehicles[0];
                else
                    return new EHApi.Vehicle();
            }
        }

        public EHApi.Card Card
        {
            get
            {
                if (DefaultCardIndex >= 0 && DefaultCardIndex < Cards.Count)
                    return Cards[DefaultCardIndex];
                else if (Cards.Count > 0)
                    return Cards[0];
                else 
                    return null;
            }
        }

        public EHLogin(EHApi api)
        {
            _api = api;
            IsLoggedIn = false;
            DefaultVehicleIndex = 0;
            DefaultCardIndex = 0;
        }

		public async Task<bool> LoginWithPassword(string username, string password, string deviceId)
        {
            var token = await _api.tokenAsync(username, password, deviceId);

            Username = username;
            Token = token;
            DeviceId = deviceId;

            if (token == null)
            {
                IsLoggedIn = false;
                return false;
            }

            return await Login2();
        }

		public async Task<bool> LoginWithToken(string username, string refreshToken, string deviceId)
		{
			var token = await _api.tokenAsync(refreshToken, deviceId);

			Username = username;
			Token = token;
			DeviceId = deviceId;

			if (token == null)
			{
				IsLoggedIn = false;
				return false;
			}

			return await Login2();
		}
		
        private async Task<bool> Login2()
        {
            var account = await _api.userAsync();

			if (account == null)
			{
				IsLoggedIn = false;
				return false;
			}
			
            Account = account;
            IsLoggedIn = true;

            Vehicles = await _api.getUserVehicleListAsync();
            Cards = await _api.getCardListAsync();

            return true;
        }

        public void Logout()
        {
            Username = "";
            Token = null;
            Account = null;
            Vehicles = null;
            Cards = null;
            IsLoggedIn = false;
        }
    }
}
