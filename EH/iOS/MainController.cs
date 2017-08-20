using System;
using UIKit;

namespace ClockworkHighway.iOS
{
	public partial class MainController : UITabBarController
	{
		bool loggedIn;

		public MainController(IntPtr handle) : base(handle)
		{
			loggedIn = false;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			if (!loggedIn)
				ShowLogin();
		}

		private void ShowLogin()
		{
			var alert = UIAlertController.Create("Login", "Enter your credentials", UIAlertControllerStyle.Alert);
			var login = UIAlertAction.Create("Login", UIAlertActionStyle.Default, (Action) => { LoggedIn(alert.TextFields[0].Text, alert.TextFields[1].Text); });

			alert.AddAction(login);

			alert.AddTextField((obj) => { obj.Placeholder = "Username"; });
			alert.AddTextField((obj) => { obj.Placeholder = "Password"; obj.SecureTextEntry = true; });

			PresentViewController(alert, true, null);
		}

		private void LoggedIn(string username, string password)
		{
			SelectedViewController.View.SetNeedsDisplay();
		}
	}
}
