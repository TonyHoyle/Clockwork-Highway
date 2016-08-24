using Android.App;

namespace ClockworkHighway.Android
{
    [Activity(Label = "@string/ApplicationName")]
    public class SearchActivity : BaseActivity
    {
        public SearchActivity() : base(typeof(SearchFragment), Resource.Layout.mainmenusearch)
        { 
        }
    }
}

