using Android.OS;
using Android.Support.V4.App;
using Android.Views;

namespace EH.Android
{
    public class AccountDetailsFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.AccountDetails, container, false);
        }
    }
}