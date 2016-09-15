using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TonyHoyle.EH
{
    public class EHLogin
    {
        public bool IsLoggedIn { get; private set; }
        public List<EHApi.Vehicle> Vehicles { get; private set; }
        public List<EHApi.Card> Cards { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public EHApi.AccountData Account { get; private set; }
        public int DefaultVehicleIndex { get; set; }
        public int DefaultCardIndex { get; set; }
        public EHApi Api { get; private set; }

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

        public EHLogin(HttpClient client)
        {
            Api = new EHApi(client);
            IsLoggedIn = false;
            DefaultVehicleIndex = 0;
            DefaultCardIndex = 0;
        }

        public async Task<bool> Login(string username, string password)
        {
            var account = await Api.loginAsync(username, password);

            if(account == null)
            {
                IsLoggedIn = false;
                return false;
            }

            Account = account;
            Username = username;
            Password = password;
            IsLoggedIn = true;

            Vehicles = await Api.getUserVehicleListAsync(username, password);
            Cards = await Api.getCardListAsync(username, password);

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
