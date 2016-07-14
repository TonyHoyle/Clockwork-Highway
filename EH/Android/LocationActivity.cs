using Android.OS;
using Android.App;
using Android.Support.V7.App;
using Android.Support.V7.Widget;

namespace EH.Android
{
    [Activity(Label = "@string/ApplicationName")]
    public class LocationActivity : BaseActivity
    {
        public LocationActivity() : base(new LocationFragment())
        {
        }
    }
}

