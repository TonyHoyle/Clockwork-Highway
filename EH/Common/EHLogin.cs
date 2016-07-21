using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EH.Common
{
    public class EHLogin
    {
        private EHApi _api;

        public bool IsLoggedIn { get; private set; }
        public List<EHApi.Vehicle> Vehicles { get; private set; }
        public List<EHApi.Card> Cards { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public EHApi.AccountData Account { get; private set; }
        public int DefaultVehicleIndex { get; set; }
        public int DefaultCardIndex { get; set; }

        public EHApi.Vehicle Vehicle {
            get
            {
                if (DefaultVehicleIndex >= 0 && DefaultVehicleIndex < Vehicles.Count)
                    return Vehicles[DefaultVehicleIndex];
                else
                    return Vehicles[0];
            }
        }

        public EHApi.Card Card
        {
            get
            {
                if (DefaultCardIndex >= 0 && DefaultCardIndex < Cards.Count)
                    return Cards[DefaultCardIndex];
                else
                    return Cards[0];
            }
        }

        public EHLogin()
        {
            _api = new EHApi();
            IsLoggedIn = false;
            DefaultVehicleIndex = 0;
            DefaultCardIndex = 0;
        }

        public async Task<bool> Login(string username, string password)
        {
            var account = await _api.loginAsync(username, password);

            if(account == null)
            {
                IsLoggedIn = false;
                return false;
            }

            Account = account;
            Username = username;
            Password = password;
            IsLoggedIn = true;

            Vehicles = await _api.getUserVehicleListAsync(username, password);
            Cards = await _api.getCardListAsync(username, password);

            return true;
        }

        public void Logout()
        {
            Username = "";
            Password = "";
            Account = null;
            Vehicles = null;
            Cards = null;
            IsLoggedIn = false;
        }
    }
}
