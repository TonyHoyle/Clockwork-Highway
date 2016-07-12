using Android.Support.V4.App;
using Android.OS;
using Android.App;

namespace EH.Android
{
    [Activity(Label = "@string/ApplicationName")]
    public class SearchActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (bundle == null)
                SupportFragmentManager.BeginTransaction().Add(global::Android.Resource.Id.Content, new SearchFragment()).Commit();
        }
    }

}

