using Android.OS;
using Android.App;
using Android.Support.V7.App;
using Android.Support.V7.Widget;

namespace EH.Android
{
    [Activity(Label = "@string/account")]
    public class AccountDetailsActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.emptylayout);

            if (bundle == null)
            {
                Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                SetSupportActionBar(toolbar);

                var fragment = new AccountDetailsFragment();
                fragment.Arguments = Intent.Extras;

                SupportFragmentManager.BeginTransaction().Add(Resource.Id.content_frame, fragment).Commit();
            }
        }
    }
}

