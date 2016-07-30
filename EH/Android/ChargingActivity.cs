using Android.OS;
using Android.App;
using Android.Support.V7.App;
using Android.Support.V7.Widget;

namespace EH.Android
{
    [Activity(Label = "@string/charging")]
    public class ChargingActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.EmptyLayout);

            if (bundle == null)
            {
                Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                SetSupportActionBar(toolbar);

                var fragment = new ChargingFragment();
                fragment.Arguments = Intent.Extras;

                SupportFragmentManager.BeginTransaction().Add(Resource.Id.content_frame, fragment).Commit();
            }
        }
    }
}

