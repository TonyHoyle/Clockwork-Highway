using System;
using UIKit;
using CoreFoundation;

namespace ClockworkHighway.iOS
{
    public class Toaster
    {
        const int NSEC_PER_SEC = 1000000000;

        static public void Toast(UIViewController owner, string message, int duration = 3)
        {
            var alert = UIAlertController.Create(null, message, UIAlertControllerStyle.ActionSheet);
            owner.PresentViewController(owner, true, null);
            DispatchQueue.MainQueue.DispatchAfter(new DispatchTime(DispatchTime.Now, duration * NSEC_PER_SEC),
                          () => { alert.DismissViewController(true, null); });
        }
    }
}
