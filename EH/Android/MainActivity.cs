using Android.Support.V4.App;
using Android.OS;
using Android.App;
using Android.Content.PM;

// 'Car with cog' logo by Benjamin STAWARZ licensed under CC BY 3.0

namespace EH.Android
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if(bundle == null)
                SupportFragmentManager.BeginTransaction().Add(global::Android.Resource.Id.Content, new MainFragment()).Commit();
        }
    }

}

