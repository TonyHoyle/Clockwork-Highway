using Android.OS;
using Android.App;
using Android.Content.PM;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using HockeyApp.Android;
using HockeyApp.Android.Metrics;

namespace ClockworkHighway.Android
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
                SetSupportActionBar(toolbar);

            if (bundle == null)
            {
                var fragment = new MainFragment();
                SupportFragmentManager.BeginTransaction().Add(global::Android.Resource.Id.Content, fragment).Commit();
            }

            MetricsManager.Register(Application, "5766f95fdc9c49b2bceb70915fa2a0e4");
        }

		protected override void OnResume()
		{
			base.OnResume();
			CrashManager.Register(this, "5766f95fdc9c49b2bceb70915fa2a0e4", new HockeyCrashManagerSettings());
		}
	}

    public class HockeyCrashManagerSettings : CrashManagerListener 
    { 
        public override bool ShouldAutoUploadCrashes() 
        { 
            return true; 
        } 
    }
}

