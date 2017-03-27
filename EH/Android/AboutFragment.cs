using Android.Support.V4.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace ClockworkHighway.Android
{
    public class AboutFragment : DialogFragment
    {
        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
			View dialogView = Activity.LayoutInflater.Inflate(Resource.Layout.aboutdialog, null);
			TextView version = (TextView)dialogView.FindViewById(Resource.Id.version);
			version.Text = "Version "+Activity.PackageManager.GetPackageInfo(Activity.PackageName, 0).VersionName;
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity)
                .SetTitle(Resource.String.ApplicationName)
                .SetView(dialogView)
                .SetPositiveButton(Resource.String.ok, (sender, args) => { Dismiss(); });
            return builder.Create();
        }
    }
}