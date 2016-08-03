using Android.App;

namespace ClockworkHighway.Android
{
    [Activity(Label = "@string/ApplicationName")]
    public class SearchActivity : BaseActivity
    {
        public SearchActivity() : base(new SearchFragment(), Resource.Layout.mainmenusearch)
        { 
        }
    }
}

