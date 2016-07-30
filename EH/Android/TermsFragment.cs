using Android.Support.V4.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using EH.Common;

namespace EH.Android
{
    public class TermsFragment : DialogFragment
    {
        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            // Calling GetLayoutInflator for the dialog here causes a recursive loop as DialogFragment.GetLayoutInflator
            // contains a call to OnCreateDialog (which seems bogus but unfixed in latest android).
            var view = Activity.LayoutInflater.Inflate(Resource.Layout.TermsAndConditions, null);
            var terms = view.FindViewById<TextView>(Resource.Id.termsAndConditions);

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity)
                .SetTitle(Resource.String.termsLong)
                .SetView(view)
                .SetPositiveButton(Resource.String.ok, (sender, args) => { });
            var dlg = builder.Create();
            loadTerms(dlg, terms);
            return dlg;
        }

        private async void loadTerms(AlertDialog dlg, TextView text)
        {
            try
            {
                text.Text = "Loading terms..";

                EHApi.Terms terms = await SharedData.login.Api.getTermsAsync();

                if (terms != null)
                    text.Text = terms.terms;
            }
            catch(EHApi.EHApiException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }
    }
}