using Foundation;
using UIKit;

namespace ClockworkHighway.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        private UIWindow window;

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            window = new UIWindow(UIScreen.MainScreen.Bounds);

            var controller = new MainScreen();
            window.RootViewController = controller;
            window.MakeKeyAndVisible();
            return true;
        }
   	}
}


