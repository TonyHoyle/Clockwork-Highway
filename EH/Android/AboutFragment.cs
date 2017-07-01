using Android.Support.V4.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Net;

namespace ClockworkHighway.Android
{
    public class AboutFragment : DialogFragment
    {
        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
			View dialogView = Activity.LayoutInflater.Inflate(Resource.Layout.aboutdialog, null);
			TextView version = dialogView.FindViewById<TextView>(Resource.Id.version);
            TextView privacy = dialogView.FindViewById<TextView>(Resource.Id.privacy);
			version.Text = "Version "+Activity.PackageManager.GetPackageInfo(Activity.PackageName, 0).VersionName;
            privacy.Click += (sender, e) => { OpenPrivacyPage(); };

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity)
                .SetTitle(Resource.String.ApplicationName)
                .SetView(dialogView)
                .SetPositiveButton(Resource.String.ok, (sender, args) => { Dismiss(); });
            return builder.Create();
        }

        private void OpenPrivacyPage()
        {
			Intent i = new Intent(Intent.ActionView);
			i.SetData(Uri.Parse("http://www.hoyle.me.uk/privacy.html"));
			StartActivity(i);
        }
    }
}