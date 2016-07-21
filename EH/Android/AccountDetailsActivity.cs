using Android.OS;
using Android.App;
using Android.Support.V7.App;
using Android.Support.V7.Widget;

namespace EH.Android
{
    [Activity(Label = "@string/account")]
    public class AccountDetailsActivity : BaseActivity
    {
        public AccountDetailsActivity() : base(new AccountDetailsFragment())
        {
        }
    }
}

