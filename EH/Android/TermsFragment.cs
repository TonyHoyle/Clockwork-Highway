using Android.Support.V4.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using TonyHoyle.EH;

namespace ClockworkHighway.Android
{
    public class TermsFragment : DialogFragment
    {
        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            // Calling GetLayoutInflator for the dialog here causes a recursive loop as DialogFragment.GetLayoutInflator
            // contains a call to OnCreateDialog (which seems bogus but unfixed in latest android).
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.termsandconditions, null);
            var terms = view.FindViewById<TextView>(Resource.Id.termsAndConditions);

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity)
//                .SetTitle(SharedData.settings.terms.title)
                .SetView(view)
                .SetPositiveButton(Resource.String.ok, (sender, args) => { });
            var dlg = builder.Create();
            terms.Text = SharedData.settings.terms.terms;
            return dlg;
        }
    }
}