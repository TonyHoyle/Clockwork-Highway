using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Foundation;
using TonyHoyle.EH;
using UIKit;

namespace ClockworkHighway.iOS
{
	public partial class MainController : UITabBarController
	{
		bool loggedIn;
        NSUserDefaults prefs;

		public MainController(IntPtr handle) : base(handle)
		{
			loggedIn = false;
			prefs = NSUserDefaults.StandardUserDefaults;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			if (!loggedIn)
				ShowLogin();
		}

		private async void ShowLogin()
		{
    		try
    		{
				Shared.httpClient = new HttpClient(new NSUrlSessionHandler());
				Shared.httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
				Shared.httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
				Shared.httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ClockworkHighway", NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString()));
				Shared.httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");

				Shared.api = new EHApi(Shared.httpClient);

				Shared.settings = await Shared.api.getSettingsAsync();
    		}
    		catch (EHApi.EHApiException e)
    		{
    			Console.WriteLine("Unable to get settings - " + e.Message);
                var dlg = UIAlertController.Create("Unable to connect",
                                                   "Unable to communicate with Electric Highway servers.  Check that you are connected to the internet and try again later.",
                                                   UIAlertControllerStyle.Alert);
                dlg.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (obj) => { DismissViewController(true, null); }));
                PresentViewController(dlg, true, null);
    			return;
    		}

            var username = prefs.StringForKey("username");
            var token = prefs.StringForKey("token");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(username))
                ShowLoginDialog(username);
            else
            {
                if (!await Shared.api.Login.LoginWithToken(username, token, UIDevice.CurrentDevice.IdentifierForVendor.ToString()))
                    ShowLoginDialog(username);
                else
                    LoggedIn();
            }
		}

        private void ShowLoginDialog(string username)
        {
			var alert = UIAlertController.Create("Login", "Enter your credentials", UIAlertControllerStyle.Alert);
			var login = UIAlertAction.Create("Login", UIAlertActionStyle.Default, (Action) => { LoginPressed(alert.TextFields[0].Text, alert.TextFields[1].Text); });

			alert.AddAction(login);

            alert.AddTextField((obj) => { obj.Placeholder = "Username"; if (!string.IsNullOrEmpty(username)) obj.Text = username; });
			alert.AddTextField((obj) => { obj.Placeholder = "Password"; obj.SecureTextEntry = true; });

			PresentViewController(alert, true, null);
		}

		private async void LoginPressed(string username, string password)
		{
            if (!await Shared.api.Login.LoginWithPassword(username, password, UIDevice.CurrentDevice.IdentifierForVendor.ToString()))
                ShowLogin();
            else
                LoggedIn();
		}

        private void LoggedIn()
        {
            prefs.SetString(Shared.api.Login.Username, "username");
            prefs.SetString(Shared.api.Login.Token.refresh_token, "token");
            prefs.Synchronize();

            NSNotificationCenter.DefaultCenter.PostNotificationName("LoggedIn", null);
        }
	}
}
