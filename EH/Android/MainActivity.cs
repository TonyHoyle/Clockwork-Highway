using Android.OS;
using Android.App;
using Android.Content.PM;
using Android.Support.V7.App;
using Android.Support.V7.Widget;

namespace EH.Android
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (bundle == null)
            {
                Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                    SetSupportActionBar(toolbar);

                var fragment = new MainFragment();
                SupportFragmentManager.BeginTransaction().Add(global::Android.Resource.Id.Content, fragment).Commit();
            }

        }
    }

}

