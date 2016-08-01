using Android.Support.V4.App;
using Android.OS;
using Android.Support.V7.App;
using TonyHoyle.EH;
using Android.Support.Design.Widget;
using Android.Views;

namespace ClockworkHighway.Android
{
    public class ForgotUsernameFragment : DialogFragment
    {
        private TextInputEditText _email;

        public override global::Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            View view = Activity.LayoutInflater.Inflate(Resource.Layout.forgotusername, null);
            _email = view.FindViewById<TextInputEditText>(Resource.Id.email);

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity)
                .SetTitle(Resource.String.forgotUsernameTitle)
                .SetView(view)
                .SetPositiveButton(Resource.String.ok, (sender, args) => { DoForgot(); })
                .SetNegativeButton(Resource.String.cancel, (sender, args) => { Dismiss(); });
            return builder.Create();
        }

        private async void DoForgot()
        {
            var eh = new EHApi(SharedData.httpClient);
            try
            {
                await eh.forgottenUsernameAsync(_email.Text);
            }
            catch (EHApi.EHApiException e)
            {
                System.Diagnostics.Debug.WriteLine("Failed to do forget username: " + e.Message);
            }
        }
    }
}