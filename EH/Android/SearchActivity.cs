using Android.Support.V7.App;
using Android.OS;
using Android.App;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Android.Views;

namespace EH.Android
{
    [Activity(Label = "@string/ApplicationName")]
    public class SearchActivity : BaseActivity
    {
        public SearchActivity() : base(new SearchFragment())
        { 
        }
    }
}

