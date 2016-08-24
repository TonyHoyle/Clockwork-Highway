using Android.OS;
using Android.App;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;

namespace ClockworkHighway.Android
{
    [Activity(Label = "@string/charging")]
    public class ChargingActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.childactivity);

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            if (bundle == null)
            {
                var fragment = new ChargingFragment();
                fragment.Arguments = Intent.Extras;

                SupportFragmentManager.BeginTransaction().Add(Resource.Id.content_frame, fragment).Commit();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}

