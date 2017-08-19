using System;
using UIKit;

namespace ClockworkHighway.iOS
{
    public class MainScreen : UITabBarController
    {
        UIViewController chargers, settings;
        bool loggedIn;

        public MainScreen()
        {
			loggedIn = false;

			chargers = new UIViewController();
            chargers.Title = "Charge";

			settings = new UIViewController();
            settings.TabBarItem = new UITabBarItem(UITabBarSystemItem.More, 0);

            var tabs = new UIViewController[] { chargers, settings };
            ViewControllers = tabs;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if(!loggedIn)
                ShowLogin();
        }

		private void ShowLogin()
		{
			var alert = UIAlertController.Create("Login", "Enter your credentials", UIAlertControllerStyle.Alert);
			var login = UIAlertAction.Create("Login", UIAlertActionStyle.Default, (Action) => { });

			alert.AddAction(login);

			alert.AddTextField((obj) => { obj.Placeholder = "Username"; });
			alert.AddTextField((obj) => { obj.Placeholder = "Password"; obj.SecureTextEntry = true; });

			PresentViewController(alert, true, null);
		}
	}
}
